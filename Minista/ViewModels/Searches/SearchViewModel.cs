using InstagramApiSharp;
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

using static Helper;
#pragma warning disable IDE0044	
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
#pragma warning disable CS0618	
namespace Minista.ViewModels.Searches
{
    public class SearchViewModel : BaseModel
    {
        public ObservableCollection<InstaDynamicSearchSection> TopItems { get; set; } = new ObservableCollection<InstaDynamicSearchSection>();

        List<InstaDiscoverRecentSearchesItem> RecentSearches { get; set; } = new List<InstaDiscoverRecentSearchesItem>();

        public ObservableCollection<InstaUser> AccountSearches { get; set; } = new ObservableCollection<InstaUser>();

        public List<InstaUser> AccountSearchesPrivate = new List<InstaUser>();

        public ObservableCollection<InstaHashtag> HashtagSearches { get; set; } = new ObservableCollection<InstaHashtag>();
        public List<InstaHashtag> HashtagSearchesPrivate = new List<InstaHashtag>();
        public ObservableCollection<InstaPlaceShort> PlaceSearches { get; set; } = new ObservableCollection<InstaPlaceShort>();


        bool RecentSearchesAdded = false;

        public void RunLoadMore()
        {
            RecentSearches.Clear();
            TopItems.Clear();
            GetDynamicSearches();
            GetRecentSearches();
        }
        public async void GetRecentSearches()
        {
            try
            {
                RecentSearchesAdded = false;
                Views.Searches.SearchView.Current?.ShowTopLoadingTop();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var result = await InstaApi.DiscoverProcessor.GetRecentSearchesAsync();
                    if(result.Succeeded)
                    {
                        AccountSearchesPrivate.Clear();
                        HashtagSearchesPrivate.Clear();
                        AccountSearches.Clear();
                        RecentSearches.Clear();
                        RecentSearches.AddRange(result.Value.Recent);
                        if (TopItems.Count > 0)
                        {
                            AddRecentSearches();
                            if (result.Value.Recent.Count > 0)
                            {
                                var xxx = result.Value.Recent.Where(a => !a.IsHashtag).Select(b => b.User).ToList();
                                xxx.ForEach(tx => AccountSearchesPrivate.Add(tx.ToUser()));
                                AccountSearches.AddRange(AccountSearchesPrivate);
                            }
                            if (result.Value.Recent.Count > 0)
                            {
                                var xxx = result.Value.Recent.Where(a => a.IsHashtag).Select(b => b.Hashtag.ToHashtag()).ToList();
                                xxx.ForEach(tx => HashtagSearchesPrivate.Add(tx));
                                HashtagSearches.AddRange(HashtagSearchesPrivate);
                            }
                        }
                    }

                    Views.Searches.SearchView.Current?.HideTopLoadingTop();
                });
            }
            catch { Views.Searches.SearchView.Current?.HideTopLoadingTop(); }
        }
        public async void GetDynamicSearches()
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var result = await InstaApi.DiscoverProcessor.GetDynamicSearchesAsync();
                    if (result.Succeeded)
                    {
                        TopItems.Clear();
                        TopItems.AddRange(result.Value.Sections);
                        AddRecentSearches();
                    }
                });
            }
            catch { }
        }
        void AddRecentSearches()
        {
            try
            {
                if (RecentSearchesAdded)
                    return;
                if (RecentSearches.Count == 0)
                    return;
                var recent = new InstaDynamicSearchSection
                {
                    Title = "Recent",
                    Type = InstagramApiSharp.Enums.InstaDynamicSearchSectionType.Recent,
                  
                };
                foreach (var item in RecentSearches)
                {
                    item.ShowClose = false;
                    recent.Items.Add(item);
                }
                //recent.Items.AddRange(RecentSearches);

                TopItems.Add(recent);
                RecentSearchesAdded = true;
            }
            catch{ }
        }
        public void RemoveItem(long pk)
        {
            try
            {
                var t = TopItems.ToList();
                for (int i = 0; i < t.Count; i++)
                {
                    var lst = t[i].Items.ToList();
                    for (int j = 0; j < lst.Count; j++)
                    {
                        if (lst[j].User?.Pk == pk)
                        {
                            t[i].Items.RemoveAt(j);

                            if (t[i].Items.Count == 0)
                                TopItems.Remove(t[i]);
                            break;
                        }
                    }
                }
            }
            catch { }
        }




        #region Accounts search
        //public string TextSearch = string.Empty;
        //PaginationParameters AccountPagination = PaginationParameters.MaxPagesToLoad(1);
        public async void SearchAccounts(string text)
        {
            try
            {
                if (text.Contains("@"))
                    text = text.Replace("@", "");
                if (text.Contains("#"))
                    text = text.Replace("#", "");
                text = text.ToLower();

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Views.Searches.SearchView.Current?.ShowTopLoadingAccount();
                    var result = await InstaApi.DiscoverProcessor.SearchPeopleAsync(text, PaginationParameters.MaxPagesToLoad(1));

                    AccountSearches.Clear();
                    if (result.Succeeded && result.Value?.Users?.Count > 0)
                    {
                        //AccountSearches.AddRange(result.Value.Users);
                        var users = result.Value.Users;//.OrderBy(x=> x.FriendshipStatus!= null && x.FriendshipStatus.Following).ToList();

                        var userIds = new List<long>();
                        if (users?.Count > 0)
                        {
                            users.ForEach(x =>
                            {
                                userIds.Add(x.Pk);
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
                                            var t = users.FirstOrDefault(u => u.Pk == x.Pk);
                                            if (t != null)
                                                t.FriendshipStatus = x;
                                        });
                                        var orderedUsers = users.OrderBy(x => x.FriendshipStatus != null && !x.FriendshipStatus.Following).ToList();

                                        //var aaaaaaa0 = users.OrderBy(x => x.FriendshipStatus != null && x.FriendshipStatus.Following).ToList();


                                        orderedUsers.ForEach(x =>
                                        {
                                            AccountSearches.Add(x);
                                        });
                                    }
                                });
                            }
                        }
                        catch { }
                    }


                    Views.Searches.SearchView.Current?.HideTopLoadingAccount();
                });
            }
            catch
            {
                Views.Searches.SearchView.Current?.HideTopLoadingAccount();
            }
        }
        #endregion

        #region Tags search

        public string TagSearch = string.Empty;
        bool MoreTagAvailable = true, IsHashtagLoading = false;
        List<long> ExcludeTagsList = new List<long>();
        public ScrollViewer HashtagScrollViewer;
        public void SetHashtagScrollViewer(ListView lv)
        {
            if (HashtagScrollViewer != null)
                return;
            HashtagScrollViewer = lv.FindScrollViewer();
            if (HashtagScrollViewer != null)
                HashtagScrollViewer.ViewChanging += HashtagScrollViewerViewChanging;
        }
        public async void SearchTags(string text = null)
        {
            var has = ExcludeTagsList.Any();
            try
            {
                if (text.Contains("@"))
                    text = text.Replace("@", "");
                if (text.Contains("#"))
                    text = text.Replace("#", "");
                text = text.ToLower();
                if (TagSearch != text)
                {
                    MoreTagAvailable = true;
                    ExcludeTagsList.Clear();
                }
                TagSearch = text;
                if (!MoreTagAvailable)
                    return;
                has = ExcludeTagsList.Any();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    HashtagSearches.Clear();
                    if(!has)
                        Views.Searches.SearchView.Current?.ShowTopLoadingTag();
                    else
                        Views.Searches.SearchView.Current?.ShowBottomLoadingTag();
                    var result = await InstaApi.HashtagProcessor.SearchHashtagAsync(text, ExcludeTagsList);
                    if (result.Succeeded)
                    {
                        if (result.Value.Count > 0)
                        {
                            HashtagSearches.AddRange(result.Value);
                            result.Value.ForEach(x => ExcludeTagsList.Add(x.Id));
                        }
                        MoreTagAvailable = result.Value.MoreAvailable;
                    }
                    if (!has)
                        Views.Searches.SearchView.Current?.HideTopLoadingTag();
                    else
                        Views.Searches.SearchView.Current?.HideBottomLoadingTag();
                });
            }
            catch
            {
                if (!has)
                    Views.Searches.SearchView.Current?.HideTopLoadingTag();
                else
                    Views.Searches.SearchView.Current?.HideBottomLoadingTag();
            }
            IsHashtagLoading = false;
        }
        private void HashtagScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (HashtagSearches == null)
                    return;
                if (!HashtagSearches.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsHashtagLoading == false)
                {
                    IsHashtagLoading = true;
                    SearchTags(TagSearch);
                }
            }
            catch (Exception ex) { ex.PrintException("HashtagScrollViewerViewChanging"); }
        }

        #endregion

        #region Place search

        public string PlaceSearch = string.Empty;
        bool MorePlaceAvailable = true, IsPlaceLoading = false;
        PaginationParameters PlacePagination = PaginationParameters.MaxPagesToLoad(1);
        public ScrollViewer PlaceScrollViewer;
        public void SetPlaceScrollViewer(ListView lv)
        {
            if (PlaceScrollViewer != null)
                return;
            PlaceScrollViewer = lv.FindScrollViewer();
            if (PlaceScrollViewer != null)
                PlaceScrollViewer.ViewChanging += PlaceScrollViewerViewChanging;
        }
        public async void SearchPlaces(string text = null)
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
            var has = PlaceSearch != text;
            try
            {
                if (has)
                {
                    MorePlaceAvailable = true;

                    PlaceSearches.Clear();
                    PlacePagination = PaginationParameters.MaxPagesToLoad(1);
                }
                PlaceSearch = text;
                if (!MorePlaceAvailable)
                    return;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (has)
                        Views.Searches.SearchView.Current?.ShowTopLoadingPlace();
                    else
                        Views.Searches.SearchView.Current?.ShowBottomLoadingPlace();
                    var result = await InstaApi.LocationProcessor.SearchPlacesAsync(text, PlacePagination);
                    if (result.Succeeded)
                    {
                        if (result.Value.Items.Count > 0)
                        {
                            var lst = new List<InstaPlaceShort>();
                            result.Value.Items.ForEach(x => lst.Add(x.Location));

                            PlaceSearches.AddRange(lst);
                        }
                        MorePlaceAvailable = result.Value.HasMore;
                    }


                    if (has)
                        Views.Searches.SearchView.Current?.HideTopLoadingPlace();
                    else
                        Views.Searches.SearchView.Current?.HideBottomLoadingPlace();
                });
            }
            catch
            {
                if (has)
                    Views.Searches.SearchView.Current?.HideTopLoadingPlace();
                else
                    Views.Searches.SearchView.Current?.HideBottomLoadingPlace();
            }
            IsPlaceLoading = false;
        }
        private void PlaceScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (PlaceSearches == null)
                    return;
                if (!PlaceSearches.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsPlaceLoading == false)
                {
                    IsPlaceLoading = true;
                    SearchPlaces(PlaceSearch);
                }
            }
            catch (Exception ex) { ex.PrintException("PlaceScrollViewerViewChanging"); }
        }

        #endregion

    }
}
