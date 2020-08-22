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
    public class FollowRequestsViewModel : BaseModel
    {
        public ObservableCollection<UserShortFriendshipWithCategory> Items { get; set; } = new ObservableCollection<UserShortFriendshipWithCategory>();
        private UserShortFriendshipWithCategory FollowRequests = CreateFollowRequests();
        private UserShortFriendshipWithCategory Suggestions = CreateSuggestions();

        static UserShortFriendshipWithCategory CreateFollowRequests() => new UserShortFriendshipWithCategory { Title = /*""*/ "Follow requests" };
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
                Views.Infos.FollowRequestsView.Current?.ShowTopLoading();
                var result = await Helper.InstaApi.UserProcessor.GetPendingFriendRequestsAsync();

                if (refresh)
                {
                    Items.Clear();
                    FollowRequests = CreateFollowRequests();
                    Suggestions = CreateSuggestions();
                }
                if (!result.Succeeded)
                {
                    Views.Infos.FollowRequestsView.Current?.HideTopLoading();
                    return;
                }
                if (result.Value.Users.Count > 0)
                    FollowRequests.Items.AddRange(result.Value.Users);
                if(result.Value.SuggestedUsers?.Count > 0)
                    Suggestions.SuggestionItems.AddRange(result.Value.SuggestedUsers);

                if (FollowRequests?.Items?.Count > 0)
                {
                    if (!CategoryExists())
                        Items.Insert(0, FollowRequests);
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

                //var users = result.Value.Select(x => x.Pk);
                //var statuses = await Helper.InstaApi.UserProcessor.GetFriendshipStatusesAsync(users.ToArray());
                //($"users count: {users.Count()}").PrintDebug();
                //($"statuses count: {statuses.Value.Count}").PrintDebug();
                //foreach (var item in statuses.Value)
                //{
                //    try
                //    {
                //        var u = Items.SingleOrDefault(s => s.Pk == item.Pk);
                //        if (u != null)
                //            u.FriendshipStatus = item;
                //    }
                //    catch { }
                //}
            }
            catch
            {
            }
            Views.Infos.FollowRequestsView.Current?.HideTopLoading();
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
                var t = FollowRequests?.Items?.ToList();
                for (int i = 0; i < t.Count; i++)
                {
                    if (t[i].Pk == pk)
                    {
                        Items.RemoveAt(i);
                        return;
                    }
                }
            }
            catch { }
        }
    }
    public class UserShortFriendshipWithCategory : BaseModel
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged("Title"); } }
        public ObservableCollection<InstaUserShortFriendship> Items { get; set; } = new ObservableCollection<InstaUserShortFriendship>();
        
        public ObservableCollection<InstaSuggestionItem> SuggestionItems { get; set; } = new ObservableCollection<InstaSuggestionItem>();
    }
}
namespace Minista.Converters
{
    class FollowRequestsWithCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ViewModels.Infos.UserShortFriendshipWithCategory Item)
            {
                if (Item.Title.ToLower().Contains("suggest"))
                    return Item.SuggestionItems;
                else
                    return Item.Items;
            }
            return new ObservableCollection<ViewModels.Infos.UserShortFriendshipWithCategory>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
