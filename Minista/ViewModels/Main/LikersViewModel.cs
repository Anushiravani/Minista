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
namespace Minista.ViewModels.Main
{
    public class LikersViewModel : BaseModel
    {
        public ObservableCollection<InstaUserShortFriendship> Items { get; set; } = new ObservableCollection<InstaUserShortFriendship>();

        InstaMedia _instaMedia;
        public InstaMedia Media
        {
            get { return _instaMedia; }
            set { _instaMedia = value; OnPropertyChanged("Media"); }
        }
        public bool HasMoreItems { get; set; } = true;

        //ScrollViewer Scroll;
        //PullToRefreshListView LV;
        public void SetMedia(InstaMedia instaMedia)
        {
            Media = instaMedia;
        }

        public async void ResetCache()
        {
            try
            {
                Media = null;
                Items.Clear();
                HasMoreItems = true;
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
            try
            {
                Views.Main.LikersView.Current?.ShowTopLoading();
                var result = await InstaApi.MediaProcessor.GetMediaLikersAsync(Media.InstaIdentifier);

                Items.Clear();
                if (!result.Succeeded)
                {
                    Views.Main.LikersView.Current?.HideTopLoading();
                    return;
                }
                if (result.Value != null && result.Value.Any())
                {
                    Items.AddRange(result.Value.Select(t=> t.ToUserShortFriendship()).ToList());
                }
                else
                {
                    Views.Main.LikersView.Current?.HideTopLoading();
                    return;
                }

                var users = result.Value.Select(x => x.Pk);
                var statuses = await InstaApi.UserProcessor.GetFriendshipStatusesAsync(users.ToArray());
                ($"users count: {users.Count()}").PrintDebug();
                ($"statuses count: {statuses.Value.Count}").PrintDebug();
                foreach(var item in statuses.Value)
                {
                    try
                    {
                        var u = Items.SingleOrDefault(s => s.Pk == item.Pk);
                        if (u != null)
                            u.FriendshipStatus = item;
                    }
                    catch { }
                }
                //for (int i = 0; i < statuses.Value.Count; i++)
                //{
                //    try
                //    {
                //        // code
                //        Items[i].FriendshipStatus = statuses.Value[0];
                //    }
                //    catch { }
                //}
            }
            catch { }
            Views.Main.LikersView.Current?.HideTopLoading();
        }

    }
}
