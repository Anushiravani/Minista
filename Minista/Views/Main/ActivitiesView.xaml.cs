﻿using System;
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
using InstagramApiSharp.Classes.Models;
using Minista.ViewModels.Main;
using Minista.Models;
using Minista.Helpers;
using Windows.UI.Xaml.Documents;
using Minista.Converters;
using Minista.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;

namespace Minista.Views.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivitiesView : Page
    {
        bool First = true;

        public ActivitiesViewModel ActivitiesVM { get; set; } = new ActivitiesViewModel();
        readonly DateTimeConverter DateConverter = new DateTimeConverter();
        public static ActivitiesView Current;
        public ActivitiesView()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += ActivitiesViewLoaded;
            DataContext = ActivitiesVM;
        }

        private void ActivitiesViewLoaded(object sender, RoutedEventArgs e)
        {

            //SetLVs();
            if (First)
            {
                ActivitiesVM.RunLoadMore(true);
                //ActivitiesVM.RunLoadMore2(true);
                First = false;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            KeyDown += OnKeyDownHandler;
            if (e.NavigationMode == NavigationMode.New)
                GetType().RemovePageFromBackStack();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            KeyDown -= OnKeyDownHandler;
        }
        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == Windows.System.VirtualKey.F5)
                    ActivitiesVM?.Refresh();
            }
            catch { }
        }
        //void SetLVs()
        //{
        //    try
        //    {
        //        ActivitiesVM.SetLV(FollowingItemsLV.FindScrollViewer());
        //    }
        //    catch { }
        //}
        private void ItemsLVRefreshRequested(object sender, EventArgs e)
        {
            ActivitiesVM?.Refresh();
        }
        private void NonFollowersItemsLVRefreshRequested(object sender, EventArgs e)
        {
            ActivitiesVM?.NonFollowersVM?.RunLoadMore(true);
        }

        private void LikedTaggedGridViewItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem != null &&
                    e.ClickedItem is InstaActivityMedia activityMedia && activityMedia != null)
                {
                    NavigationService.Navigate(typeof(Posts.SinglePostView), activityMedia.Id);
                }
            }
            catch { }
        }

        private async void FollowUnfollowHashtagButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && 
                    btn.DataContext is RecentActivityFeed recentActivity && recentActivity != null)
                {
                    if (recentActivity.StoryType == InstagramApiSharp.Enums.InstaActivityFeedStoryType.LikedTagged &&
                        recentActivity.HashtagFollow != null)
                    {
                        if (recentActivity.HashtagFollow.FollowStatus)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                var result = await Helper.InstaApi.HashtagProcessor.UnFollowHashtagAsync(recentActivity.HashtagFollow.Name);
                                if (result.Succeeded)
                                    recentActivity.HashtagFollow.FollowStatus = false;
                            });
                        }
                        else
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                var result = await Helper.InstaApi.HashtagProcessor.FollowHashtagAsync(recentActivity.HashtagFollow.Name);
                                if (result.Succeeded)
                                    recentActivity.HashtagFollow.FollowStatus = true;
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private void TextBlockDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            try
            {
                if (sender is TextBlock textBlock && args.NewValue is RecentActivityFeed recentActivity && recentActivity != null)
                {
                    if (recentActivity.StoryType == InstagramApiSharp.Enums.InstaActivityFeedStoryType.LikedTagged)
                    {
                        using (var pg = new PassageHelperX())
                        {
                            var passages = pg.GetInlines(recentActivity.RichText, HyperLinkHelper.HyperLinkClick);
                            textBlock.Inlines.Clear();
                            passages.Item1.ForEach(item =>
                            textBlock.Inlines.Add(item));
                        }
                    }
                    else
                    {
                        var textStr = recentActivity.Text;
                        var text = recentActivity.Text;

                        if (recentActivity.Links?.Count > 0)
                        {
                            foreach (var link in recentActivity.Links)
                            {
                                try
                                {
                                    var mention = textStr.Substring(link.Start, (link.End - link.Start) -1);
                                    //if (link.Type == InstagramApiSharp.Enums.InstaLinkType.User)
                                    //    text = text.Replace(mention, $"@{mention}");//⥽⍬⥶⍬⥽
                                    //else
                                        text = text.Replace(mention, $"https:\\{mention}".Replace(" ", "⥽⍬⥶⍬⥽"));//⥽⍬⥶⍬⥽
                                }
                                catch { }
                            }
                        }

                        using (var pg = new PassageHelperX())
                        {
                            var passages = pg.GetInlines(text, HyperLinkHelper.HyperLinkClick);
                            textBlock.Inlines.Clear();
                            passages.Item1.ForEach(item =>
                            textBlock.Inlines.Add(item));
                        }
                    }

                    textBlock.Inlines.Add(new LineBreak());
                    var date = "";
                    try
                    {
                        date = (string)DateConverter.Convert(recentActivity.TimeStamp, typeof(DateTime), null, null);
                    }
                    catch { date = ""; }
                    if (!string.IsNullOrEmpty(date))
                    {
                        textBlock.Inlines.Add(new Run
                        {
                            Foreground = (SolidColorBrush)Application.Current.Resources["DefaultInnerForegroundColor"],
                            Text = date,
                           
                        });
                    }
                }
            }
            catch { }
        }
        private void MediaImageExTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is ImageEx imageEx && imageEx.DataContext is RecentActivityFeed recentActivity && recentActivity != null)
                {
                    if (recentActivity.Medias?.Count > 0)
                        NavigationService.Navigate(typeof(Posts.SinglePostView), recentActivity.Medias[0].Id);

                }
            }
            catch { }
        }

        private void UserEllipseTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Ellipse ellipse && ellipse.DataContext is RecentActivityFeed recentActivity && recentActivity != null)
                {
                    if (!string.IsNullOrEmpty(recentActivity.ProfileName))
                        Helper.OpenProfile(recentActivity.ProfileName);
                    else if(recentActivity.ProfileId > 0)
                        Helper.OpenProfile(recentActivity.ProfileId);
                }
            }
            catch { }
        }

        //private void FollowingItemsLVRefreshRequested(object sender, EventArgs e)
        //{
        //    ActivitiesVM?.Refresh2();
        //}

        private void MainPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    SetLVs();
            //}
            //catch { }
        }

        //private void FollowingItemsLVLoaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        SetLVs();
        //    }
        //    catch { }
        //}

        private void FriendRequestTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(typeof(Infos.FollowRequestsView));
            }
            catch { }
        }

        private void UserTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid gid && gid.DataContext is InstaUserShortFriendship item && item != null)
                {
                    Helper.OpenProfile(item.ToUserShort());
                }
            }
            catch { }
        }
        private async void FollowUnFollowMainButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn != null && btn.DataContext is InstaUserShortFriendship user)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (btn.Content.ToString() == "Follow")
                        {
                            var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(user.Pk);
                            if (result.Succeeded)
                                user.FriendshipStatus = result.Value?.ToFriendshipShortStatus();
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to  @{user.UserName}.\r\n" +
                                      $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        else if (btn.Content.ToString() == "Unfollow" || btn.Content.ToString() == "Requested")
                        {
                            var result = await Helper.InstaApi.UserProcessor.UnFollowUserAsync(user.Pk);
                            if (result.Succeeded)
                                user.FriendshipStatus = result.Value?.ToFriendshipShortStatus();
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to  @{user.UserName}.\r\n" +
                                     $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                    });
                }
            }
            catch { }
        }

        #region LOADINGS You
        public void ShowTopLoadingYou() => TopLoadingYou.Start();
        public void HideTopLoadingYou() => TopLoadingYou.Stop();


        public void ShowBottomLoadingYou() => BottomLoadingYou.Start();
        public void HideBottomLoadingYou() => BottomLoadingYou.Stop();



        public void ShowTopLoadingFollowers() { /*TopLoadingFollowers.Start(); */}
        public void HideTopLoadingFollowers() { /*TopLoadingFollowers.Stop();*/ }
        #endregion LOADINGS You
         

        //#region LOADINGS Following
        //public void ShowTopLoadingFollowing() => TopLoadingFollowing.Start();
        //public void HideTopLoadingFollowing() => TopLoadingFollowing.Stop();


        //public void ShowBottomLoadingFollowing() => BottomLoadingFollowing.Start();
        //public void HideBottomLoadingFollowing() => BottomLoadingFollowing.Stop();
        //#endregion LOADINGS Following
    }
}
