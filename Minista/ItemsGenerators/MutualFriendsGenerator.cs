using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Minista.ViewModels.Infos;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using static Helper;
#pragma warning disable IDE0044	
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
#pragma warning disable CS0618	
namespace Minista.ItemsGenerators
{
    public class MutualFriendsGenerator : BaseModel, IGenerator
    {
        public bool FirstRun = true;
        public ObservableCollection<UserShortFriendshipWithCategory> Items { get; set; } = new ObservableCollection<UserShortFriendshipWithCategory>();
        private UserShortFriendshipWithCategory Multuals = CreateMutuals();
        private UserShortFriendshipWithCategory Suggestions = CreateSuggestions();

        static UserShortFriendshipWithCategory CreateMutuals() => new UserShortFriendshipWithCategory { Title = "Followed by you" };
        static UserShortFriendshipWithCategory CreateSuggestions() => new UserShortFriendshipWithCategory { Title = "Suggestions for you" };

        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);

        public long UserId { get; private set; }
        private int PageCount = 1;
        bool IsLoading = true;
        //bool IsMyUser = false;
        public void SetUserId(long pk)
        {
            if (pk != UserId)
            {
                Items.Clear();
                FirstRun = true;
                Pagination = PaginationParameters.MaxPagesToLoad(1);
                HasMoreItems = true;
                IsLoading = true;
            }
            UserId = pk;
            //IsMyUser = pk == CurrentUser.Pk;
            if (IsLoading) { }
        }
        public async void RunLoadMore(bool refresh = false)
        {
            //if (!IsMyUser) return;
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

                    Multuals = CreateMutuals();
                    Suggestions = CreateSuggestions();

                    Views.Infos.FollowView.Current?.ShowTopLoadingMutual();
                }
                else
                    Views.Infos.FollowView.Current?.ShowBottomLoadingMutual();
                var result = await InstaApi.UserProcessor.GetMutualFriendsOrSuggestionAsync(UserId);

                PageCount++;
                FirstRun = false;
                Pagination.MaximumPagesToLoad = 2;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value.MutualFollowers?.Count == 0)
                    {
                        if (refresh)
                            Views.Infos.FollowView.Current?.HideTopLoadingMutual();
                        else
                            Views.Infos.FollowView.Current?.HideBottomLoadingMutual();
                        return;
                    }
                }

                HasMoreItems = false;

                if (refresh) Items.Clear();
                var userIds = new List<long>();
                var userIdsX = new List<long>();
                if (result.Value?.MutualFollowers?.Count > 0)
                {
                    result.Value.MutualFollowers.ForEach(x =>
                    {
                        userIds.Add(x.Pk);
                        Multuals.Items.Add(x.ToUserShortFriendship());
                    });
                }
                if (result.Value?.SuggestedUsers?.Count > 0)
                {
                    result.Value.SuggestedUsers.ForEach(x =>
                    {
                        userIdsX.Add(x.User.Pk);
                        Suggestions.SuggestionItems.Add(x);
                    });
                }
                if (Multuals?.Items?.Count > 0)
                {
                    if (!CategoryExists())
                        Items.Insert(0, Multuals);
                }
                if (Suggestions?.SuggestionItems?.Count > 0)
                {
                    if (!CategoryExists(true))
                        Items.Add(Suggestions);
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
                                    var t = Multuals.Items.FirstOrDefault(u => u.Pk == x.Pk);
                                    if (t != null)
                                        t.FriendshipStatus = x;
                                });
                            }
                        });
                    }
                }
                catch { }

                try
                {
                    if (userIdsX.Count > 0)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            var friendshipStatuses = await InstaApi.UserProcessor.GetFriendshipStatusesAsync(userIdsX.ToArray());
                            if (friendshipStatuses.Succeeded)
                            {
                                var friends = friendshipStatuses.Value;
                                friends.ForEach(x =>
                                {
                                    var t = Suggestions.Items.FirstOrDefault(u => u.Pk == x.Pk);
                                    if (t != null)
                                        t.FriendshipStatus = x;
                                });
                            }
                        });
                    }
                }
                catch { }
                await Task.Delay(1000);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                FirstRun =
                   IsLoading = false;
                ex.PrintException("MutualFriendsGenerator.LoadMoreItemsAsync");
            }

            if (refresh)
                Views.Infos.FollowView.Current?.HideTopLoadingMutual();
            else
                Views.Infos.FollowView.Current?.HideBottomLoadingMutual();
        }

        bool CategoryExists(bool suggestions = false)
        {
            if (Items.Count == 0) return false;
            return Items.Any(x => suggestions ? x.Title.ToLower().Contains("suggest") : x.Title.ToLower().Contains("follow"));
        }
    }
}
