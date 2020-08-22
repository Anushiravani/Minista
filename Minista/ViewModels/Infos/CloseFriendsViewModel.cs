using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;
using Minista.Views.Infos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Minista.ViewModels.Infos
{
    public class CloseFriendsViewModel : BaseModel
    {
        public bool FirstRun = true;
        public ObservableCollection<InstaUserShortFriendship> Items { get; set; } = new ObservableCollection<InstaUserShortFriendship>();
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
                    CloseFriendsView.Current?.ShowTopLoading();
                }
                else
                    CloseFriendsView.Current?.ShowBottomLoading();
                var result = await Helper.InstaApi.UserProcessor.GetBestFriendsAsync(Pagination);

                FirstRun = false;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value?.Count == 0)
                    {
                        if (refresh)
                            CloseFriendsView.Current?.HideTopLoading();
                        else
                            CloseFriendsView.Current?.HideBottomLoading();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(result.Value.NextMaxId))
                    HasMoreItems = false;

                Pagination.NextMaxId = result.Value.NextMaxId;
                if (refresh) Items.Clear();
                var userIds = new List<long>();
                if (result.Value?.Count > 0)
                {
                    result.Value.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        Items.Add(x.ToUserShortFriendship());
                    });
                }
                await Task.Delay(1000);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("CloseFriendsViewModel.LoadMoreItemsAsync");
            }

            if (refresh)
                CloseFriendsView.Current?.HideTopLoading();
            else
                CloseFriendsView.Current?.HideBottomLoading();
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
            catch (Exception ex) { ex.PrintException("ScrollViewChanging"); }
        }











        public void AddItem(InstaUserShortFriendship user)
        {
            try
            {
                if (user == null) return;
                var flag = Items.Any(x => x.Pk == user.Pk);
                if (!flag)
                    Items.Add(user);
            }
            catch { }
        }










        public string SearchText = null;
        public ObservableCollection<InstaUserShortFriendship> SearchItems { get; set; } = new ObservableCollection<InstaUserShortFriendship>();

        public async void RunLoadMoreSearch(string text = null)
        {
            await RunLoadMoreSearchAsync(text);
        }
        public async Task RunLoadMoreSearchAsync(string text = null)
        {
            await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsSearchAsync(text);
            });
        }
        async Task LoadMoreItemsSearchAsync(string text = null)
        {
            try
            {
                if (text.Contains("@"))
                    text = text.Replace("@", "");
                if (text.Contains("#"))
                    text = text.Replace("#", "");
                text = text.ToLower();
            }
            catch { }
            try
            {
                    SearchItems.Clear();
                SearchText = text;
                BlockedView.Current?.ShowTopLoading();
                var result = await Helper.InstaApi.DiscoverProcessor.SearchPeopleAsync(SearchText, PaginationParameters.MaxPagesToLoad(1), 50);

                if (!result.Succeeded)
                {
                    if (result.Value == null || result.Value?.Users?.Count == 0)
                    {
                        BlockedView.Current?.HideTopLoading();
                        return;
                    }
                }
                var userIds = new List<long>();
                if (result.Value?.Users?.Count > 0)
                {
                    result.Value.Users.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        SearchItems.Add(x.ToUserShortFriendship());
                    });
                }

                try
                {
                    if (userIds.Count > 0)
                    {
                        await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            var friendshipStatuses = await Helper.InstaApi.UserProcessor.GetFriendshipStatusesAsync(userIds.ToArray());
                            if (friendshipStatuses.Succeeded)
                            {
                                var friends = friendshipStatuses.Value;
                                friends.ForEach(x =>
                                {
                                    var t = SearchItems.FirstOrDefault(u => u.Pk == x.Pk);
                                    if (t != null)
                                    {
                                        t.FriendshipStatus = x;
                                        t.IsBestie = x.IsBestie;
                                    }
                                });
                            }
                        });
                    }
                }
                catch { }
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                ex.PrintException("CloseFriendsViewModel.LoadMoreItemsAsync2");
            }

            BlockedView.Current?.HideTopLoading();
        }














        public bool FirstRun2 = true;
        public ObservableCollection<InstaUserShortFriendship> SuggestionItems { get; set; } = new ObservableCollection<InstaUserShortFriendship>();
        public bool HasMoreItems2 { get; set; } = true;
        public PaginationParameters Pagination2 { get; private set; } = PaginationParameters.MaxPagesToLoad(1);
        ScrollViewer Scroll2;
        bool IsLoading2 = true;

        public void SetLV2(ScrollViewer scrollViewer)
        {
            Scroll2 = scrollViewer;

            if (Scroll2 != null)
            {
                Scroll2.ViewChanging += ScrollViewChanging2;
                HasMoreItems2 = true;
                IsLoading2 = true;
            }
        }
        public async void RunLoadMore2(bool refresh = false)
        {
            await RunLoadMoreAsync2(refresh);
        }
        public async Task RunLoadMoreAsync2(bool refresh = false)
        {
            await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
                    CloseFriendsView.Current?.ShowTopLoadingSuggestion();
                }
                else
                    CloseFriendsView.Current?.ShowBottomLoadingSuggestion();
                var result = await Helper.InstaApi.UserProcessor.GetBestFriendsSuggestionsAsync(Pagination);

                FirstRun2= false;
                if (!result.Succeeded)
                {
                    IsLoading2 = false;
                    if (result.Value == null || result.Value?.Count == 0)
                    {
                        if (refresh)
                            CloseFriendsView.Current?.HideTopLoadingSuggestion();
                        else
                            CloseFriendsView.Current?.HideBottomLoadingSuggestion();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(result.Value.NextMaxId))
                    HasMoreItems2 = false;

                Pagination2.NextMaxId = result.Value.NextMaxId;
                if (refresh) SuggestionItems.Clear();
                var userIds = new List<long>();
                if (result.Value?.Count > 0)
                {
                    result.Value.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        SuggestionItems.Add(x.ToUserShortFriendship());
                    });
                }
                await Task.Delay(1000);
                IsLoading2 = false;
            }
            catch (Exception ex)
            {
                FirstRun2 =
                   IsLoading2 = false;
                ex.PrintException("CloseFriendsViewModel.LoadMoreItemsAsync2");
            }

            if (refresh)
                CloseFriendsView.Current?.HideTopLoadingSuggestion();
            else
                CloseFriendsView.Current?.HideBottomLoadingSuggestion();
        }

        private void ScrollViewChanging2(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (SuggestionItems == null)
                    return;
                if (!SuggestionItems.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading2 == false && !FirstRun2)
                {
                    IsLoading2 = true;
                    RunLoadMore2();
                }
            }
            catch (Exception ex) { ex.PrintException("ScrollViewChanging2"); }
        }


    }
}
