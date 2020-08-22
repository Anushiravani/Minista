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
using Minista.ViewModels.Infos;
using Windows.UI.Core;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using System.Numerics;


namespace Minista.Views.Infos
{
    public sealed partial class RecentFollowersView : Page
    {
        private bool IsLoadedBefore = false;
        private readonly Visual _refreshButtonVisual;

        readonly Compositor _compositor;
        public static RecentFollowersView Current;
        public RecentFollowersView()
        {
            this.InitializeComponent();
            Current = this;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _refreshButtonVisual = RefreshButton.GetVisual();

            //DataContext = RecentFollowersVM;
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += RecentFollowersViewLoaded;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            KeyDown += OnKeyDownHandler;
            if (e.NavigationMode == NavigationMode.New)
            {
                GetType().RemovePageFromBackStack();
                IsLoadedBefore = false;
            }
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
                    RefreshControl.RequestRefresh();
            }
            catch { }
        }
        private void RecentFollowersViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshControl.RefreshRequested -= RefreshControlRefreshRequested;
                RefreshControl.Visualizer.RefreshStateChanged -= RefreshControlRefreshStateChanged;
            }
            catch { }
            RefreshControl.RefreshRequested += RefreshControlRefreshRequested;
            if (RefreshControl.Visualizer != null)
                RefreshControl.Visualizer.RefreshStateChanged += RefreshControlRefreshStateChanged;
            if (!IsLoadedBefore)
            {
                IsLoadedBefore = true;
                RecentFollowersVM.RunLoadMore(true);
            }
        }

        private void RefreshControlRefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            using (var RefreshCompletionDeferral = args.GetDeferral())
                RecentFollowersVM.RunLoadMore(true);
        }
        private void RefreshControlRefreshStateChanged(Microsoft.UI.Xaml.Controls.RefreshVisualizer sender, Microsoft.UI.Xaml.Controls.RefreshStateChangedEventArgs args)
        {
            if (args.NewState == Microsoft.UI.Xaml.Controls.RefreshVisualizerState.Refreshing)
            {
                RefreshButton.IsEnabled = false;
            }
            else
            {
                RefreshButton.IsEnabled = true;
            }
        }
        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            RefreshControl.RequestRefresh();
        }

        private async void DismissUserClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarButton btn && btn.DataContext is InstaSuggestionItem item && item != null)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        RecentFollowersVM.RemoveItemFromSuggestions(item.User.Pk);
                        var result = await Helper.InstaApi.DiscoverProcessor.DismissUserSuggestionAsync(item.User.Pk.ToString());
                    });
                }
            }
            catch { }
        }

        private void UserTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid grid && grid.DataContext is InstaUserShortFriendship user && user != null)
                    Helper.OpenProfile(user);
            }
            catch { }
        }

        private void ItemsLVRefreshRequested(object sender, EventArgs e)
        {
            try
            {
                RecentFollowersVM.RunLoadMore(true);
            }
            catch { }
        }

        private void SuggestionTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid grid && grid.DataContext is InstaSuggestionItem item && item != null)
                    Helper.OpenProfile(item.User);
            }
            catch { }
        }

        private async void FollowButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is InstaSuggestionItem item && item != null)
                {
                    if (btn.Content.ToString().ToLower() == "follow")
                    {
                        var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(item.User.Pk);
                        if (result.Succeeded)
                        {
                            var data = result.Value;
                            if (data.Following)
                                btn.Content = "Unfollow";
                            else if (data.OutgoingRequest)
                                btn.Content = "Requested";
                            else
                                btn.Content = "Follow";
                        }
                        else
                        {
                            if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                            else
                                Helper.ShowNotify($"Error while sending follow request to  @{item.User.UserName}.\r\n" +
                                  $"Error message: {result.Info?.Message}", 2000);
                        }
                    }
                    else if (btn.Content.ToString() == "unfollow" || btn.Content.ToString() == "requested")
                    {
                        var result = await Helper.InstaApi.UserProcessor.UnFollowUserAsync(item.User.Pk);
                        if (result.Succeeded)
                        {
                            var data = result.Value;
                            if (data.Following)
                                btn.Content = "Unfollow";
                            else if (data.OutgoingRequest)
                                btn.Content = "Requested";
                            else
                                btn.Content = "Follow";
                        }
                        else
                        {
                            if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                            else
                                Helper.ShowNotify($"Error while sending unfollow request to  @{item.User.UserName}.\r\n" +
                                  $"Error message: {result.Info?.Message}", 2000);
                        }
                    }
                }
            }
            catch { }
        }

        private void ItemsLVLoaded(object sender, RoutedEventArgs e)
        {
            var scroll = ItemsLV.FindScrollViewer();
            if (scroll != null)
                scroll.ViewChanging += ScrollViewViewChanging;
        }
        private async void FollowClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn != null)
                {
                    if (btn.DataContext is InstaUserShortFriendship data && data != null)
                    {
                        $"{data.UserName}  {data.Pk}".PrintDebug();
                        var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(data.Pk);
                        if (result.Succeeded)
                        {
                            var x = result.Value;
                            data.FriendshipStatus = new InstaFriendshipShortStatus
                            {
                                Following = x.Following,
                                IncomingRequest = x.IncomingRequest,
                                IsPrivate = x.IsPrivate,
                                OutgoingRequest = x.OutgoingRequest
                            };
                            btn.DataContext = data;
                        }
                        else
                        {
                            switch (result.Info.ResponseType)
                            {
                                case InstagramApiSharp.Classes.ResponseType.RequestsLimit:
                                case InstagramApiSharp.Classes.ResponseType.SentryBlock:
                                    result.Info.Message.ShowMsg("ERR");
                                    break;
                                case InstagramApiSharp.Classes.ResponseType.ActionBlocked:
                                    "Action blocked.\r\nPlease try again 5 or 10 minutes later".ShowMsg("ERR");
                                    break;
                            }
                        }
                    }
                }
            }
            catch { }
        }


        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;




        private void ScrollViewViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                var scrollViewer = sender as ScrollViewer;
                if ((scrollViewer.VerticalOffset - _lastVerticalOffset) > 5 && !_isHideTitleGrid)
                {
                    _isHideTitleGrid = true;
                    //ToggleGoUpButtonAnimation(false);
                    ToggleRefreshButtonAnimation(false);
                }
                else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
                {
                    _isHideTitleGrid = false;
                    //ToggleGoUpButtonAnimation(true);
                    ToggleRefreshButtonAnimation(true);
                }
                if (scrollViewer.VerticalOffset == 0)
                {
                    _isHideTitleGrid = true;
                    //ToggleGoUpButtonAnimation(false);
                    ToggleRefreshButtonAnimation(false);
                }
                _lastVerticalOffset = scrollViewer.VerticalOffset;
            }
            catch { }
        }

        private void ToggleRefreshButtonAnimation(bool show)
        {
            try
            {
                var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
                scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
                scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

                _refreshButtonVisual.CenterPoint = new Vector3((float)RefreshButton.ActualWidth / 2f, (float)RefreshButton.ActualHeight / 2f, 0f);
                _refreshButtonVisual.StartAnimation("Scale.X", scaleAnimation);
                _refreshButtonVisual.StartAnimation("Scale.Y", scaleAnimation);
            }
            catch { }
        }




        #region LOADINGS
        public void ShowTopLoading() => TopLoading.Start();
        public void HideTopLoading() => TopLoading.Stop();
        #endregion LOADINGS

    }
}
