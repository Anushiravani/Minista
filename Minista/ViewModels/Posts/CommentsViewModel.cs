using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;
using Minista.ItemsGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Helper;
namespace Minista.ViewModels.Posts
{
    public class CommentsViewModel : BaseModel, IGenerator
    {
        public bool FirstRun = true;
        public ObservableCollection<InstaComment> Items { get; set; } = new ObservableCollection<InstaComment>();
        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; }

        public string MediaId { get; set; }
        private InstaMedia _media;
        public InstaMedia Media
        {
            get { return _media; }
            set
            {
                _media = value;
                OnPropertyChanged("Media");
            }
        }
        ScrollViewer Scroll;
        bool IsLoading = true;
        private InstaComment _replyComment;
        public InstaComment ReplyComment
        {
            get { return _replyComment; }
            set
            {
                _replyComment = value;
                OnPropertyChanged("ReplyComment");
                ReplyVisibility = value == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private Visibility _replyVisibility = Visibility.Collapsed;
        public Visibility ReplyVisibility
        {
            get { return _replyVisibility; }
            set { _replyVisibility = value; OnPropertyChanged("ReplyVisibility"); }
        }
        public async void ResetCache()
        {
            try
            {
                _replyVisibility = Visibility.Collapsed;
                FirstRun = true;
                IsLoading = true;
                ReplyComment = null;
                Media = null;
                Items.Clear();
                HasMoreItems = true;
                Pagination = PaginationParameters.MaxPagesToLoad(2);
                await Task.Delay(350);
            }
            catch { }
        }
        public void SetLV(ScrollViewer scrollViewer)
        {
            Scroll = scrollViewer;

            if (Scroll != null)
                Scroll.ViewChanging += Scroll_ViewChanging;
            HasMoreItems = true;
            IsLoading = true;
            Pagination = PaginationParameters.MaxPagesToLoad(2);
        }
        public void SetMedia(InstaMedia media)
        {
            Media = media;
            SetMedia(media.InstaIdentifier);
            try
            {
                if (media?.Caption != null)
                    Items.Add(media.Caption?.ToComment());
            }
            catch { }
        }
        public void SetMedia(string id)
        {
            MediaId = id;
            GetMedia();
        }
        async void GetMedia()
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var mediaResult = await InstaApi.MediaProcessor.GetMediaByIdAsync(MediaId);
                    if (mediaResult.Succeeded)
                        Media = mediaResult.Value;
                });
            }
            catch { }
        }
        public async void RunLoadMore(bool refresh = false)
        {
            await RunLoadMoreAsync(refresh);
        }
        public async Task RunLoadMoreAsync(bool refresh = false)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync(refresh);
            });
        }
        async Task LoadMoreItemsAsync(bool refresh = false)
        {
            if (!HasMoreItems && !refresh)
            {
                IsLoading = false;
                return;
            }
            try
            {
                if (refresh)
                {
                    Pagination = PaginationParameters.MaxPagesToLoad(2);
                    Views.Posts.CommentView.Current?.ShowTopLoading();
                }
                else
                    Views.Posts.CommentView.Current?.ShowBottomLoading();
                var result = await InstaApi.CommentProcessor.GetMediaCommentsAsync(MediaId, Pagination);
                FirstRun = false;
                Pagination.MaximumPagesToLoad = 1;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value?.Comments?.Count == 0)
                    {
                        if (refresh)
                            Views.Posts.CommentView.Current?.HideTopLoading();
                        else
                            Views.Posts.CommentView.Current?.HideBottomLoading();
                        return;
                    }
                }

                if (result.Value.NextMaxId == null && result.Value.NextMinId == null)
                    HasMoreItems = false;

                //Pagination.NextMaxId = result.Value.NextMaxId;
                //Pagination.NextMinId = result.Value.NextMinId;
                if (result.Value?.Comments?.Count > 0)
                {
                    if (refresh)
                    {
                        Items.Clear();
                        try
                        {
                            if (Media?.Caption != null)
                            {
                                var first = Media.Caption.ToComment();
                                first.IsCommentsDisabled = Media.IsCommentsDisabled;
                                Items.Add(first);
                            }
                        }
                        catch { }
                    }

                    result.Value.Comments.ForEach(item =>
                    {
                        item.IsCommentsDisabled = Media.IsCommentsDisabled;
                        Items.Add(item);
                    });
                }
                await Task.Delay(1000);
                IsLoading = false;

            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("UserDetailsMediasGenerator.LoadMoreItemsAsync");
            }
            if (refresh)
                Views.Posts.CommentView.Current?.HideTopLoading();
            else
                Views.Posts.CommentView.Current?.HideBottomLoading();
            refresh = false;
        }
        public void Refresh()
        {
            RunLoadMore(true);
        }
        private async void Scroll_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (Items == null)
                    return;
                if (!Items.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading == false && !FirstRun)
                {
                    IsLoading = true;
                    await RunLoadMoreAsync();
                }
            }
            catch (Exception ex) { ex.PrintException("Scroll_ViewChanging"); }
        }


        public async void GetTailCommentReplies(InstaComment comment)
        {
            try
            {
                if (!comment.HasMoreTailChildComments)
                    return;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (comment.PaginationParameters == null)
                        comment.PaginationParameters = PaginationParameters.MaxPagesToLoad(1);

                    var result = await InstaApi.CommentProcessor.GetMediaRepliesCommentsAsync(MediaId, comment.Pk.ToString(), comment.PaginationParameters);
                    if (result.Succeeded)
                    {
                        if (result.Value.ChildComments?.Count > 0)
                        {
                            result.Value.ChildComments.Reverse();
                            result.Value.ChildComments.ForEach(cmt =>
                            {
                                cmt.IsCommentsDisabled = Media.IsCommentsDisabled;
                                comment.ChildComments.Insert(0, cmt);
                            });
                        }
                        comment.NumTailChildComments = result.Value.NumTailChildComments;
                        comment.HasMoreHeadChildComments = result.Value.HasMoreHeadChildComments;
                        comment.HasMoreTailChildComments = result.Value.HasMoreTailChildComments;
                    }

                });
            }
            catch { }
        }
        public async void GetHeadCommentReplies(InstaComment comment)
        {
            try
            {
                if (!comment.HasMoreHeadChildComments)
                    return;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (comment.PaginationParameters == null)
                        comment.PaginationParameters = PaginationParameters.MaxPagesToLoad(1);

                    var result = await InstaApi.CommentProcessor.GetMediaRepliesCommentsAsync(MediaId, comment.Pk.ToString(), comment.PaginationParameters);
                    if (result.Succeeded)
                    {
                        if (result.Value.ChildComments?.Count > 0)
                        {
                            result.Value.ChildComments.Reverse();
                            result.Value.ChildComments.ForEach(cmt => 
                            {
                                cmt.IsCommentsDisabled = Media.IsCommentsDisabled;
                                comment.ChildComments.Add(cmt);
                            });
                        }
                        comment.NumTailChildComments = result.Value.NumTailChildComments;
                        comment.HasMoreHeadChildComments = result.Value.HasMoreHeadChildComments;
                        comment.HasMoreTailChildComments = result.Value.HasMoreTailChildComments;
                    }

                });
            }
            catch { }
        }
    }
}
