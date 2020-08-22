using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using static Helper;

namespace Minista.UserControls.Story.StoryViews
{
    public class QuizAnswerViewModel : BaseModel
    {
        public bool FirstRun = true;
        public ObservableCollection<InstaStoryQuizAnswer> Items { get; set; } = new ObservableCollection<InstaStoryQuizAnswer>();
        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);

        ScrollViewer Scroll;
        bool IsLoading = true;
        public InstaStoryItem StoryItem { get; set; }
        public InstaStoryQuizStickerItem StoryQuizStickerItem { get; set; }
        public void SetItem(InstaStoryItem storyItem, InstaStoryQuizStickerItem storyQuizStickerItem, ScrollViewer scrollViewer)
        {
            ResetCache();
            StoryItem = storyItem;
            StoryQuizStickerItem = storyQuizStickerItem;
            RunLoadMore(true);
            SetLV(scrollViewer);
        }
        public void SetLV(ScrollViewer scrollViewer)
        {
            if (Scroll == null)
            {
                Scroll = scrollViewer;
                if (Scroll == null) return;
                Scroll.ViewChanging += ScrollViewChanging;
            }
            else
            {
                try
                {
                    Scroll.ViewChanging -= ScrollViewChanging;
                }
                catch { }
            }
        }
        public void ResetCache()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            IsLoading = true;
            HasMoreItems = true;
            Items.Clear();
            FirstRun = true;
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
                    Pagination = PaginationParameters.MaxPagesToLoad(1);

                }
                else
                {

                }
                var result = await InstaApi.StoryProcessor.GetStoryQuizParticipantsAsync(StoryItem.Id, StoryQuizStickerItem.QuizId.ToString(),
                    Pagination);

                FirstRun = false;
                Pagination.MaximumPagesToLoad = 1;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value.Participants?.Count == 0)
                    {
                        Hide(refresh);
                        return;
                    }
                }

                HasMoreItems = result.Value.MoreAvailable ?? false;

                Pagination.NextMaxId = result.Value.MaxId;
                if (refresh) Items.Clear();
                if (result.Value.Participants?.Count > 0)
                {
                    var obj = result.Value.Participants[0];
                    for (int i = 0; i < result.Value.Participants.Count; i++)
                    {
                        var index = result.Value.Participants[i].Answer;
                        result.Value.Participants[i].AnswerText = StoryQuizStickerItem.Tallies[index].Text;
                    }
                    Items.AddRange(result.Value.Participants);
                }
                await Task.Delay(1000);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("Participants.LoadMoreItemsAsync");
            }
            Hide(refresh);
        }
        void Hide(bool refresh)
        {

        }
        public void ScrollViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
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
