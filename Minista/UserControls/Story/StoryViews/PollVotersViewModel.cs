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
    public class PollVotersViewModel : BaseModel
    {
        ScrollViewer Scroll1, Scroll2;

        public bool FirstRun = true;
        public InstaStoryItem StoryItem { get; set; }
        public InstaStoryPollStickerItem StoryPollSticker { get; set; }

        public ObservableCollection<InstaStoryVoterItem> ItemsYes { get; set; } = new ObservableCollection<InstaStoryVoterItem>();
        public bool HasMoreItems1 { get; set; } = true;
        public PaginationParameters Pagination1 { get; private set; } = PaginationParameters.MaxPagesToLoad(1);
        bool IsLoading1 = true;

        public ObservableCollection<InstaStoryVoterItem> ItemsNo { get; set; } = new ObservableCollection<InstaStoryVoterItem>();
        public bool HasMoreItems2 { get; set; } = true;
        public PaginationParameters Pagination2 { get; private set; } = PaginationParameters.MaxPagesToLoad(1);
        bool IsLoading2 = true;

        private string _yesText = "YES";
        private string _noText = "NO";
        public string YesText { get => _yesText; set { _yesText = value; OnPropertyChanged("YesText"); } }
        public string NoText { get => _noText; set { _noText = value; OnPropertyChanged("NoText"); } }
        public void SetItem(InstaStoryItem storyItem, InstaStoryPollStickerItem pollStickerItem, ScrollViewer scrollViewer1, ScrollViewer scrollViewer2)
        {
            ResetCache();
            StoryItem = storyItem;
            StoryPollSticker = pollStickerItem;
            YesText = pollStickerItem.Tallies[0].Text;
            NoText = pollStickerItem.Tallies[1].Text;
            RunLoadMore1(true);
            RunLoadMore2(true);
            SetLV1(scrollViewer1);
            SetLV2(scrollViewer2);
        }
        public void SetLV1(ScrollViewer scrollViewer)
        {
            if (Scroll1 == null)
            {
                Scroll1 = scrollViewer;
                if (Scroll1 == null) return;
                Scroll1.ViewChanging += ScrollView1Changing;
            }
            else
            {
                try
                {
                    Scroll1.ViewChanging -= ScrollView1Changing;
                }
                catch { }
            }
        }
        public void SetLV2(ScrollViewer scrollViewer)
        {
            if (Scroll2 == null)
            {
                Scroll2 = scrollViewer;
                if (Scroll2 == null) return;
                Scroll2.ViewChanging += ScrollView2Changing;
            }
            else
            {
                try
                {
                    Scroll2.ViewChanging -= ScrollView2Changing;
                }
                catch { }
            }
        }
        public void ResetCache()
        {
            Pagination1 = PaginationParameters.MaxPagesToLoad(1);
            IsLoading1 = true;
            HasMoreItems1 = true;
            ItemsYes.Clear();


            Pagination2 = PaginationParameters.MaxPagesToLoad(1);
            IsLoading2 = true;
            HasMoreItems2 = true;
            ItemsNo.Clear();
            FirstRun = true;
        }
        public async void RunLoadMore1(bool refresh = false)
        {
            await RunLoadMoreAsync1(refresh);
        }
        public async Task RunLoadMoreAsync1(bool refresh = false)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync1(refresh);
            });
        }
        async Task LoadMoreItemsAsync1(bool refresh = false)
        {
            if (!HasMoreItems1 && !refresh)
            {
                IsLoading1 = false;
                return;
            }
            try
            {
                if (refresh)
                {
                    Pagination1 = PaginationParameters.MaxPagesToLoad(1);

                }
                else
                {

                }
                var result = await InstaApi.StoryProcessor.GetStoryPollVotersAsync(StoryItem.Id, StoryPollSticker.PollId.ToString(), 0, Pagination1);
                FirstRun = false;
                if (!result.Succeeded)
                {
                    IsLoading1 = false;
                    if (result.Value == null || result.Value.Voters?.Count == 0)
                    {
                        Hide(refresh);
                        return;
                    }
                }

                HasMoreItems1 = result.Value.MoreAvailable;

                Pagination1.NextMaxId = result.Value.MaxId;
                if (refresh) ItemsYes.Clear();
                if (result.Value.Voters?.Count > 0)
                {
                    for (int i = 0; i < result.Value.Voters.Count; i++)
                    {
                        result.Value.Voters[i].VoteYesText = StoryPollSticker.Tallies[0].Text;
                        result.Value.Voters[i].VoteNoText = StoryPollSticker.Tallies[1].Text;

                    }
                    ItemsYes.AddRange(result.Value.Voters);
                }

                await Task.Delay(1000);
                IsLoading1 = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading1 = false;
                ex.PrintException("QuestionRespondersViewModel.LoadMoreItemsAsync");
            }
            Hide(refresh);
        }
        void Hide(bool refresh)
        {

        }
        public void ScrollView1Changing(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (ItemsYes == null)
                    return;
                if (!ItemsYes.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading1 == false && !FirstRun)
                {
                    IsLoading1 = true;
                    RunLoadMore1();
                }
            }
            catch (Exception ex) { ex.PrintException("Scroll_ViewChanging"); }
        }








        public async void RunLoadMore2(bool refresh = false)
        {
            await RunLoadMoreAsync2(refresh);
        }
        public async Task RunLoadMoreAsync2(bool refresh = false)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync2(refresh);
            });
        }
        async Task LoadMoreItemsAsync2(bool refresh = false)
        {
            if (!HasMoreItems2 && !refresh)
            {
                IsLoading2 = false;
                return;
            }
            try
            {
                if (refresh)
                {
                    Pagination2 = PaginationParameters.MaxPagesToLoad(1);

                }
                else
                {

                }
                var result = await InstaApi.StoryProcessor.GetStoryPollVotersAsync(StoryItem.Id, StoryPollSticker.PollId.ToString(), 1, Pagination1);
                FirstRun = false;
                if (!result.Succeeded)
                {
                    IsLoading2 = false;
                    if (result.Value == null || result.Value.Voters?.Count == 0)
                    {
                        Hide(refresh);
                        return;
                    }
                }

                HasMoreItems2 = result.Value.MoreAvailable;

                Pagination2.NextMaxId = result.Value.MaxId;
                if (refresh) ItemsNo.Clear();
                if (result.Value.Voters?.Count > 0)
                {
                    for (int i = 0; i < result.Value.Voters.Count; i++)
                    {
                        result.Value.Voters[i].VoteYesText = StoryPollSticker.Tallies[0].Text;
                        result.Value.Voters[i].VoteNoText = StoryPollSticker.Tallies[1].Text;

                    }
                    ItemsNo.AddRange(result.Value.Voters);
                }

                await Task.Delay(1000);
                IsLoading2 = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading2 = false;
                ex.PrintException("QuestionRespondersViewModel.LoadMoreItemsAsync");
            }
            Hide(refresh);
        }
        public void ScrollView2Changing(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (ItemsYes == null)
                    return;
                if (!ItemsYes.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading2 == false && !FirstRun)
                {
                    IsLoading2 = true;
                    RunLoadMore2();
                }
            }
            catch (Exception ex) { ex.PrintException("Scroll_ViewChanging"); }
        }

    }
}
