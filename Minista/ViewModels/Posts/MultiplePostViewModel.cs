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
using Minista.Views.Posts;
namespace Minista.ViewModels.Posts
{
    public class MultiplePostViewModel : BaseModel, IGenerator
    {
        private string title_;
        public string Title { get { return title_; } set { title_ = value; OnPropertyChanged("Title"); } }
        public MultiplePostView View;
        public void SetView(MultiplePostView view)
        {
            View = view;
        }
        public string ChannelId { get; set; }
        public bool FirstRun = true;
        public ObservableCollection<InstaMedia> Items { get; set; } = new ObservableCollection<InstaMedia>();
        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);

        ScrollViewer Scroll;
        bool IsLoading = true;
        public string FirstMediaId { get; set; }
        public void SetLV(ScrollViewer scrollViewer)
        {
            Scroll = scrollViewer;

            if (Scroll != null)
            {
                Scroll.ViewChanging += ScrollViewChanging;
                HasMoreItems = true;
                IsLoading = true;
            }
        }
        public async void ResetCache()
        {
            try
            {
                HasMoreItems = true;
                IsLoading = true;
                HasMoreItems = true;
                FirstMediaId = null;
                Pagination = PaginationParameters.MaxPagesToLoad(1);
                Items.Clear();
                ChannelId = null;
                Title = null;
                await Task.Delay(350);
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
                    Pagination = PaginationParameters.MaxPagesToLoad(1);
                View?.ShowBottomLoading();
                var result = await InstaApi.HashtagProcessor.GetHashtagChannelVideosAsync(ChannelId, FirstMediaId, Pagination);

                FirstRun = false;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value?.Count == 0)
                    {
                        View?.HideBottomLoading();

                        return;
                    }
                }
                HasMoreItems = !string.IsNullOrEmpty(result.Value.NextMaxId);


                Pagination.NextMaxId = result.Value.NextMaxId;
                if (refresh) Items.Clear();
                if (result.Value?.Count > 0)
                {
                    FirstMediaId = result.Value[0].InstaIdentifier;
                    if (refresh)
                    {
                        var l = new ObservableCollection<InstaMedia>();

                        result.Value.ForEach(x =>
                        {
                            //var p = new InstaPost
                            //{
                            //    Media = x,
                            //    Type = InstagramApiSharp.Enums.InstaFeedsType.Media
                            //};
                            //l.Add(p);

                            Items.Add(x);
                        });

                        //Items = l;
                    }
                    else
                    {

                        result.Value.ForEach(x =>
                        {
                            //var p = new InstaPost
                            //{
                            //    Media = x,
                            //    Type = InstagramApiSharp.Enums.InstaFeedsType.Media
                            //};
                            //Items.Add(p);
                            Items.Add(x);
                        });
                    }
                    //Items.AddRange(result.Value);
      
                }

                await Task.Delay(1000);
                IsLoading = false;

                //SetIndex(refresh);
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("MultiplePostViewModel.LoadMoreItemsAsync");
            }
            View?.HideBottomLoading();
        }
        //async void SetIndex(bool refresh)
        //{
        //    if (!refresh) return;
        //    try
        //    {
        //        await Task.Delay(3500);
        //        if (refresh && View != null && Items.Count > 0)
        //            View.LVPosts.SelectedIndex = 0;
        //    }
        //    catch { }
        //}
        private void ScrollViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
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
                    RunLoadMore();
                }
            }
            catch (Exception ex) { ex.PrintException("Scroll_ViewChanging"); }
        }


    }
}
