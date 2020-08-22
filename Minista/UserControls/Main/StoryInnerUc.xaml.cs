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
using Minista.Helpers;
namespace Minista.UserControls.Main
{
    public sealed partial class StoryInnerUc : UserControl
    {
        public enum StoryInnerItem
        {
            Hashtag,
            MediaFeed,
            UserMention,
            Location
        }
        public StoryInnerItem StoryInner { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public StoryInnerUc(StoryInnerItem storyInner, string title, string text)
        {
            InitializeComponent();
            StoryInner = storyInner;
            Text = text;
            Title = title;
            Loaded += StoryInnerUcLoaded;
        }

        private void StoryInnerUcLoaded(object sender, RoutedEventArgs e)
        {
            TxtText.Text = Title;
        }

        private void GridTapped(object sender, TappedRoutedEventArgs e)
        {
            switch (StoryInner)
            {
                case StoryInnerItem.MediaFeed:
                    NavigationService.Navigate(typeof(Views.Posts.SinglePostView), Text);
                    break;
                case StoryInnerItem.Hashtag:
                    NavigationService.Navigate(typeof(Views.Infos.HashtagView), Text);
                    break;
                case StoryInnerItem.UserMention:
                    Helper.OpenProfile(Text);
                    break;
                case StoryInnerItem.Location:
                    "Location/Place is not implemented YET.".ShowMsg();
                    break;
            }
        
        }
    }
}
