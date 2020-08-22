using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Minista.Views.TV
{
    public sealed partial class FooterView : UserControl
    {
        public TVPlayer TVPlayer;
        public FooterView()
        {
            this.InitializeComponent();
        }

        private void LVItemsLoaded(object sender, RoutedEventArgs e)
        {

        }

        private async void LVItems_ItemClick(object sender, ItemClickEventArgs e)
        {

            try
            {
                if (e != null && e.ClickedItem is InstaMedia media && media != null)
                {
                    var index = LVItems.Items.IndexOf(media);
                    FooterVM.SelectedIndex = index;
                    TVPlayer.CustomMTC.CanShowControls = true;
                    var mediaX = FooterVM.Items[FooterVM.SelectedIndex];
                    TVPlayer.ME.Source = mediaX.Videos[0].Uri?.ToUri();
                    TVPlayer.HideFooter();
                    await System.Threading.Tasks.Task.Delay(250);
                    TVPlayer. CustomMTC.SetMedia(mediaX);
                    LVItems.ScrollIntoView(mediaX);
                }
            }
            catch { }
        }
    }
}
