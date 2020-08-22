using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Minista.Views.TV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TVPlayer : Page
    {
        public InstaUserShort User;
        public int SelectedIndex = 0;
        public readonly InstaMediaList MediaList = new InstaMediaList();
        InstaMedia Media;
        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static TVPlayer Current;
        public TVPlayer()
        {
            this.InitializeComponent();
            Current = this;
            Loaded += TVPlayer_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
                GetType().RemovePageFromBackStack();
            NavigationMode = e.NavigationMode;
            //if (e.Parameter is InstaMedia media)
            //{
            //    Media = media;
            //}
            //else
            if (e.Parameter is object[] obj)
            {
                if (obj.Length == 2)
                {
                    var mediaList = obj[0] as List<InstaMedia>;
                    SelectedIndex = (int)obj[1];
                    MediaList.Clear();
                    if (mediaList?.Count > 0)
                    {
                        MediaList.AddRange(mediaList);
                        Media = MediaList[SelectedIndex];
                    }
                }
                else if (obj.Length == 3)
                {
                    var mediaList = obj[0] as List<InstaMedia>;
                    SelectedIndex = (int)obj[1];
                    MediaList.Clear();
                    User = obj[2] as InstaUserShort;
                    if (mediaList?.Count > 0)
                    {
                        MediaList.AddRange(mediaList);
                        Media = MediaList[SelectedIndex];
                    }
                }
            }

        }
        async void A()
        {
            
            var pncm = Windows.Networking.PushNotifications.PushNotificationChannelManager.GetDefault();
            var cc = await pncm.CreatePushNotificationChannelForApplicationAsync();
            // pncm.CreateRawPushNotificationChannelWithAlternateKeyForApplicationAsync
            //     ()
        }
        private void TVPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaComments != null)
                {
                    MediaComments.BackAction = ()=> { HideComments(); };
                }
                if (NavigationMode == NavigationMode.Back && Media != null)
                {
                    if (FooterView.FooterVM?.CurrentMedia?.InstaIdentifier == Media.InstaIdentifier)
                        return;
                }
                else if (NavigationMode == NavigationMode.New)
                {
                    NavigationCacheMode = NavigationCacheMode.Enabled;
                    CanLoadFirstPopUp = false;

                }
                if (!CanLoadFirstPopUp)
                {
                    FooterView.FooterVM?.ResetCache();
                    //public InstaUserShort User;
                    //public int SelectedIndex = 0;
                    //public readonly InstaMediaList MediaList = new InstaMediaList();
                    //InstaMedia Media;
                    FooterView.TVPlayer = this;
                    if (User == null)
                        FooterView.FooterVM.SetData(MediaList, SelectedIndex);
                    else
                        FooterView.FooterVM.SetData(User, MediaList, SelectedIndex);
                    CanLoadFirstPopUp = true;
                }
            }
            catch { }
        }

        bool HasLoaded = false;
        private async void CustomMTC_Loaded(object sender, RoutedEventArgs e)
        {
            if (HasLoaded) return;
            HasLoaded = true;
            "CustomMTC_Loaded".PrintDebug();
            if (Media == null) return;
            ME.Source = Media.Videos[0].Uri?.ToUri();
            CustomMTC.TVPlayer = this;

            await Task.Delay(500);
            CustomMTC.SetMedia(Media);
        }
        public void HideComments()
        {
            MediaComments.Visibility = Visibility.Collapsed;
            CustomMTC.CanShowControls = true;
        }
        public void HideFooter()
        {
            FooterView.Visibility = Visibility.Collapsed;
            CustomMTC.CanShowControls = true;

        }

        private void MEMediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                var more = FooterView.FooterVM.SelectedIndex;
                more.PrintDebug();
                more += 1;
                FooterView.FooterVM.Items.Count.PrintDebug();
                if (more < FooterView.FooterVM.Items.Count)
                {
                    FooterView.FooterVM.SelectedIndex = more;
                    var media = FooterView.FooterVM.Items[FooterView.FooterVM.SelectedIndex];
                    ME.Source = media.Videos[0].Uri?.ToUri();

                    CustomMTC.SetMedia(media);
                    FooterView.LVItems.SelectedIndex = more;
                }
                
            }
            catch { }
        }
    }
}
