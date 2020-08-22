using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Minista.ItemsGenerators;
using Minista.ViewModels.Infos;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using static Helper;
#pragma warning disable IDE0044	
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
#pragma warning disable CS0618	
namespace Minista.ViewModels.Main
{
    public class NonFollowersViewModel : BaseModel, IGenerator
    {
        public bool FirstRun = true;
        public ObservableCollection<InstaUserShortFriendship> Items { get; set; } = new ObservableCollection<InstaUserShortFriendship>();
        public List<InstaUserShortFriendship> CachedItems { get; set; } = new List<InstaUserShortFriendship>();

        public bool HasMoreItems { get; set; } = true;

        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);

        private int PageCount = 1;
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
        public void ResetCache()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            IsLoading = true;
            HasMoreItems = true;
            Items.Clear();
            CachedItems.Clear();
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
                    PageCount = 1;
                    Pagination = PaginationParameters.MaxPagesToLoad(1);
                }
                Views.Main.ActivitiesView.Current?.ShowTopLoadingFollowers();
                var result = await InstaApi.UserProcessor.GetUserFollowingByIdAsync(CurrentUser.Pk, Pagination);

                PageCount++;
                FirstRun = false;
                Pagination.MaximumPagesToLoad = 2;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value?.Count == 0)
                    {
                        Views.Main.ActivitiesView.Current?.HideTopLoadingFollowers();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(result.Value.NextMaxId))
                    HasMoreItems = false;

                Pagination.NextMaxId = result.Value.NextMaxId;
                if (refresh) { Items.Clear(); CachedItems.Clear(); }
                var userIds = new List<long>();
                if (result.Value?.Count > 0)
                {
                    result.Value.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        CachedItems.Add(x.ToUserShortFriendship());
                    });
                }
                try
                {
                    if (userIds.Count > 0)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            var friendshipStatuses = await InstaApi.UserProcessor.GetFriendshipStatusesAsync(userIds.ToArray());
                            if (friendshipStatuses.Succeeded)
                            {
                                var friends = friendshipStatuses.Value;
                                friends.ForEach(x =>
                                {
                                    var t = CachedItems.FirstOrDefault(u => u.Pk == x.Pk);
                                    if (t != null)
                                        t.FriendshipStatus = x;
                                });
                            }
                            CachedItems.ForEach(x =>
                            {
                                if (x.FriendshipStatus != null)
                                {
                                    if (!x.FriendshipStatus.Following)
                                        Items.Add(x);
                                }
                            });

                            Views.Main.ActivitiesView.Current?.HideTopLoadingFollowers();
                        });
                    }
                }
                catch
                {
                    Views.Main.ActivitiesView.Current?.HideTopLoadingFollowers();
                }
                await Task.Delay(1000);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("FollowingGenerator.LoadMoreItemsAsync");

                Views.Main.ActivitiesView.Current?.HideTopLoadingFollowers();
            }

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
