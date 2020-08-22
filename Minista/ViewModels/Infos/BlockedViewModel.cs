using InstagramApiSharp.Classes.Models;
using Minista.Helpers;
using Minista.ItemsGenerators;
using Minista.Views.Infos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using InstagramApiSharp;
using Windows.UI.Core;

namespace Minista.ViewModels.Infos
{
    public class BlockedViewModel : BaseModel
    {
        public bool FirstRun = true;
        public ObservableCollection<InstaBlockedUserInfo> Items { get; set; } = new ObservableCollection<InstaBlockedUserInfo>();
        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);
        ScrollViewer Scroll;
        bool IsLoading = true;

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
        public async void RunLoadMore(bool refresh = false)
        {
            await RunLoadMoreAsync(refresh);
        }
        public async Task RunLoadMoreAsync(bool refresh = false)
        {
            await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
                    BlockedView.Current?.ShowTopLoading();
                }
                else
                    BlockedView.Current?.ShowBottomLoading();
                var result = await Helper.InstaApi.UserProcessor.GetBlockedUsersAsync(Pagination);

                FirstRun = false;
                //Pagination.MaximumPagesToLoad = 1;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value?.BlockedList?.Count == 0)
                    {
                        if (refresh)
                            BlockedView.Current?.HideTopLoading();
                        else
                            BlockedView.Current?.HideBottomLoading();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(result.Value.MaxId))
                    HasMoreItems = false;

                Pagination.NextMaxId = result.Value.MaxId;
                if (refresh) Items.Clear();
                var userIds = new List<long>();
                if (result.Value?.BlockedList?.Count > 0)
                {
                    result.Value.BlockedList.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        Items.Add(x);
                    });
                }
                await Task.Delay(1000);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("BlockedViewModel.LoadMoreItemsAsync");
            }

            if (refresh)
                BlockedView.Current?.HideTopLoading();
            else
                BlockedView.Current?.HideBottomLoading();
        }

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
