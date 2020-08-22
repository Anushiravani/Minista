using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

#pragma warning disable IDE0044	
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
#pragma warning disable CS0618	

namespace Minista.ViewModels.Infos
{
    public class RecentFollowersViewModel : BaseModel
    {
        public ObservableCollection<UserShortFriendshipWithCategory> Items { get; set; } = new ObservableCollection<UserShortFriendshipWithCategory>();
        private UserShortFriendshipWithCategory Followers = CreateFollowers();
        private UserShortFriendshipWithCategory Suggestions = CreateSuggestions();

        static UserShortFriendshipWithCategory CreateFollowers() => new UserShortFriendshipWithCategory { Title = /*""*/ "Followers" };
        static UserShortFriendshipWithCategory CreateSuggestions() => new UserShortFriendshipWithCategory { Title = "Suggestions" };
        public bool HasMoreItems { get; set; } = true;
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
            try
            {
                Views.Infos.RecentFollowersView.Current?.ShowTopLoading();
                var result = await Helper.InstaApi.UserProcessor.GetRecentFollowersAsync();

                if (refresh)
                {
                    Items.Clear();
                    Followers = CreateFollowers();
                    Suggestions = CreateSuggestions();
                }
                if (!result.Succeeded)
                {
                    Views.Infos.RecentFollowersView.Current?.HideTopLoading();
                    return;
                }
                if (result.Value.Users.Count > 0)
                    Followers.Items.AddRange(result.Value.Users);
                if (result.Value.SuggestedUsers?.Count > 0)
                    Suggestions.SuggestionItems.AddRange(result.Value.SuggestedUsers);

                if (Followers?.Items?.Count > 0)
                {
                    if (!CategoryExists())
                        Items.Insert(0, Followers);
                }
                if (Suggestions?.SuggestionItems?.Count > 0)
                {
                    if (!CategoryExists(true))
                        Items.Add(Suggestions);
                }
                //if (result.Value != null && result.Value.Any())
                //{
                //    Items.AddRange(result.Value.Select(t => t.ToUserShortFriendship()).ToList());
                //}
                //else
                //    return;
                try
                {
                    await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (Followers?.Items?.Count > 0)
                        {
                            try
                            {
                                var users = Followers.Items.Select(x => x.Pk);
                                var statuses = await Helper.InstaApi.UserProcessor.GetFriendshipStatusesAsync(users.ToArray());
                                foreach (var item in statuses.Value)
                                {
                                    try
                                    {
                                        var u = Followers.Items.SingleOrDefault(s => s.Pk == item.Pk);
                                        if (u != null)
                                            u.FriendshipStatus = item;
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    });
                }
                catch { }
            }
            catch
            {
            }
            Views.Infos.RecentFollowersView.Current?.HideTopLoading();
        }


        bool CategoryExists(bool suggestions = false)
        {
            if (Items.Count == 0) return false;
            return Items.Any(x => suggestions ? x.Title.ToLower().Contains("suggest") : x.Title.ToLower().Contains("follow"));
        }
        public void RemoveItem(long pk)
        {
            try
            {
                var t = Followers?.Items?.ToList();
                for (int i = 0; i < t.Count; i++)
                {
                    if (t[i].Pk == pk)
                    {
                        Followers.Items.RemoveAt(i);
                        return;
                    }
                }
            }
            catch { }
        }
        public void RemoveItemFromSuggestions(long pk)
        {
            try
            {
                var t = Suggestions?.SuggestionItems?.ToList();
                for (int i = 0; i < t.Count; i++)
                {
                    if (t[i].User.Pk == pk)
                    {
                        Suggestions.SuggestionItems.RemoveAt(i);
                        return;
                    }
                }
            }
            catch { }
        }
    }
}
