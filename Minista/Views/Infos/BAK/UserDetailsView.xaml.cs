using InstagramApiSharp.Classes.Models;
using Minista.ViewModels.Infos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace Minista.Views.Infos
{
    public sealed partial class UserDetailsView : Page
    {
        readonly Compositor _compositor;
        readonly ImplicitAnimationCollection _elementImplicitAnimation;
        InstaUserShort UserShort;
        InstaUserShortFriendship UserShortFriendship;
        private readonly Visual _goUpButtonVisual, _refreshButtonVisual;
        ScrollViewer ScrollView;
        //public UserDetailsViewModel UserDetailsVM { get; set; } = new UserDetailsViewModel();
        bool ImageAnimationExists = false;
        private string Username = null;

        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static UserDetailsView Current;
        public UserDetailsView()
        {
            this.InitializeComponent();
            Current = this;
            //DataContext = UserDetailsVM;
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
           
            // Create ImplicitAnimations Collection. 
            _elementImplicitAnimation = _compositor.CreateImplicitAnimationCollection();

            //Define trigger and animation that should play when the trigger is triggered. 

            //_elementImplicitAnimation["Offset"] = CreateOffsetAnimation();
            _elementImplicitAnimation["Opacity"] = CreateOpacityAnimation();

            _goUpButtonVisual = GoUpButton.GetVisual();
            _refreshButtonVisual = RefreshButton.GetVisual();
            Loaded += UserDetailsViewLoaded;
            LV.ScrollChanged += LVScrollChanged;
        }

        private void LVScrollChanged(object sender, ScrollViewer e)
        {
            if (ScrollView == null)
            {

                ScrollView = e;
                ScrollView.ViewChanging += ScrollViewViewChanging;

                //ToggleGoUpButtonAnimation(false);
                //ToggleRefreshButtonAnimation(false);
                //UserDetailsVM.View = this;
                //if (UserShort != null)
                //    UserDetailsVM.SetUser(UserShort);
                //else if (UserShortFriendship != null)
                //{
                //    UserDetailsVM.SetUser(UserShortFriendship);
                //    UpdateUserImage(UserShortFriendship.ProfilePicture);
                //}

                UserDetailsVM.SetScrollViewer(ScrollView);
                //UserDetailsVM.SetBiographyTextBlock(BiographyText);
            }
        }

        public void UpdateUserImage(string img)
        {
            try
            {
                UserImage.Fill = img.GetImageBrush();
            }
            catch { }
            //var media = new InstaMedia();
            //media.Carousel[0].Images[0].Uri;
        }
        public void ResetUserImage()
        {
            try
            {
                UpdateUserImage("ms-appx:///Assets/Images/transparent-img.png");
            }
            catch { }
        }
        private void UserDetailsViewLoaded(object sender, RoutedEventArgs e)
        {
            //Loaded -= UserDetailsViewLoaded;
            try
            {
                RefreshControl.RefreshRequested -= RefreshControlRefreshRequested;
                RefreshControl.Visualizer.RefreshStateChanged -= RefreshControlRefreshStateChanged;
            }
            catch { }
            RefreshControl.RefreshRequested += RefreshControlRefreshRequested;
            if (RefreshControl.Visualizer != null)
                RefreshControl.Visualizer.RefreshStateChanged += RefreshControlRefreshStateChanged;
            ToggleGoUpButtonAnimation(false);
            ToggleRefreshButtonAnimation(false);
            if (NavigationMode == NavigationMode.Back && UserDetailsVM.User != null)
            {
                if (UserDetailsVM.User?.Pk == UserShort?.Pk)
                    return;
            }
            else if (NavigationMode == NavigationMode.Back && UserDetailsVM.User != null)
            {
                if (UserDetailsVM.User?.Pk == UserShortFriendship?.Pk)
                    return;
            }
            else if (NavigationMode == NavigationMode.Back && UserDetailsVM.User != null)
            {
                if (UserDetailsVM.User?.UserName?.ToLower() == Username?.ToLower())
                    return;
            }
            else if (NavigationMode == NavigationMode.New)
            {
                GetType().RemovePageFromBackStack();
                //this.ResetPageCache();
                //NavigationCacheMode = NavigationCacheMode.Disabled;
                NavigationCacheMode = NavigationCacheMode.Enabled;
                CanLoadFirstPopUp = false;
            }
            try
            {
                if (NavigationMode == NavigationMode.New /*|| NavigationMode == NavigationMode.Refresh*/)
                {
                    if (ScrollableUserPostUc.Visibility == Visibility.Visible)
                        ScrollableUserPostUc.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
            if (!CanLoadFirstPopUp)
            {
                //if (!CanLoadFirstPopUp) return;
                UserDetailsVM.ResetToDefault();
                ResetUserImage();
                UserDetailsVM.View = this;
                if (UserShort != null)
                {
                    UserDetailsVM.SetUser(UserShort);
                    ScrollableUserPostUc.SetData(UserShort, UserDetailsVM.MediaGeneratror);
                }
                else if (UserShortFriendship != null)
                {
                    UserDetailsVM.SetUser(UserShortFriendship);
                    UpdateUserImage(UserShortFriendship.ProfilePicture);
                    ScrollableUserPostUc.SetData(UserShortFriendship.ToUserShort(), UserDetailsVM.MediaGeneratror);
                }
                else if (!string.IsNullOrEmpty(Username))
                {
                    UserDetailsVM.SetUsername(Username);
                    ScrollableUserPostUc.SetData(new InstaUserShort { UserName = Username }, UserDetailsVM.MediaGeneratror);
                }
                UserDetailsVM.SetBiographyTextBlock(BiographyText);
                CanLoadFirstPopUp = true;
            }
        }
        public void LoadExternalProfile(object e)
        {
            if (e is InstaUserShort userShort && userShort != null)
            {
                //UserDetailsVM = new UserDetailsViewModel();
                //DataContext = UserDetailsVM;
                UserShort = userShort;
                UserShortFriendship = null;
                Username = null;
                //NavigationCacheMode = NavigationCacheMode.Disabled;
                //NavigationCacheMode = NavigationCacheMode.Required;
                try
                {
                    ConnectedAnimation imageAnimation =
                        ConnectedAnimationService.GetForCurrentView().GetAnimation("UserImage");
                    if (imageAnimation != null)
                    {
                        ImageAnimationExists = true;
                        //imageAnimation.Completed += ImageAnimationClose_Completed;
                        //if (LatestGrid?.Name != InfoGrid.Name)
                        //    LatestGrid.Background = new SolidColorBrush(Helper.GetColorFromHex("#FF2E2E2E"));

                        imageAnimation.TryStart(UserImage);
                        //GridShadow.Visibility = GVSHOW.Visibility = Visibility.Collapsed;
                    }
                    else ImageAnimationExists = false;
                }
                catch { }
            }
            else if (e is List<InstaUserShortFriendship> userShortFriendshipList)
            {
                UserShortFriendship = userShortFriendshipList.FirstOrDefault();
                UserShort = null;
                Username = null;
            }
            //else if (e.Parameter is InstaUserShortFriendship userShortFriendship)
            //{
            //    UserShortFriendship = userShortFriendship;
            //    UserShort = null;
            //    Username = null;
            //}
            else if (e is string username && !string.IsNullOrEmpty(username))
            {
                Username = username;
                UserShort = null;
                UserShortFriendship = null;
            }
            else if (e is object[] obj)
            {
                //UserDetailsVM = new UserDetailsViewModel();
                //DataContext = UserDetailsVM;
                //NavigationCacheMode = NavigationCacheMode.Disabled;
                //NavigationCacheMode = NavigationCacheMode.Required;
                UserShort = obj[0] as InstaUserShort;
                Username = null;
                UserShortFriendship = null;
                try
                {
                    UserImage.Fill = obj[1] as Brush;
                    ConnectedAnimation imageAnimation =
                       ConnectedAnimationService.GetForCurrentView().GetAnimation("UserImage");
                    if (imageAnimation != null)
                    {
                        ImageAnimationExists = true;
                        //imageAnimation.Completed += ImageAnimationClose_Completed;
                        //if (LatestGrid?.Name != InfoGrid.Name)
                        //    LatestGrid.Background = new SolidColorBrush(Helper.GetColorFromHex("#FF2E2E2E"));

                        imageAnimation.TryStart(UserImage);
                        //GridShadow.Visibility = GVSHOW.Visibility = Visibility.Collapsed;
                    }
                    else
                        ImageAnimationExists = false;
                }
                catch { }
            }

            LoadData();
        }
        void LoadData()
        {
            try
            {
                UserDetailsVM.ResetToDefault();
                ResetUserImage();
                UserDetailsVM.View = this;
                if (UserShort != null)
                {
                    UserDetailsVM.SetUser(UserShort);
                    ScrollableUserPostUc.SetData(UserShort, UserDetailsVM.MediaGeneratror);
                }
                else if (UserShortFriendship != null)
                {
                    UserDetailsVM.SetUser(UserShortFriendship);
                    UpdateUserImage(UserShortFriendship.ProfilePicture);
                    ScrollableUserPostUc.SetData(UserShortFriendship.ToUserShort(), UserDetailsVM.MediaGeneratror);
                }
                else if (!string.IsNullOrEmpty(Username))
                {
                    UserDetailsVM.SetUsername(Username);
                    ScrollableUserPostUc.SetData(new InstaUserShort { UserName = Username }, UserDetailsVM.MediaGeneratror);
                }
                UserDetailsVM.SetBiographyTextBlock(BiographyText);
                CanLoadFirstPopUp = true;
            }
            catch { }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //NavigationMode = e.NavigationMode;
            //if (NavigationMode != NavigationMode.Back)
            //{
            //    NavigationCacheMode = NavigationCacheMode.Disabled;
            //    NavigationCacheMode = NavigationCacheMode.Enabled;
            //}

            KeyDown += OnKeyDownHandler;
            NavigationMode = e.NavigationMode;
            if (e != null && e.Parameter != null)
            {
                if (e.Parameter is InstaUserShort userShort && userShort != null)
                {
                    //UserDetailsVM = new UserDetailsViewModel();
                    //DataContext = UserDetailsVM;
                    UserShort = userShort;
                    UserShortFriendship = null;
                    Username = null;
                    //NavigationCacheMode = NavigationCacheMode.Disabled;
                    //NavigationCacheMode = NavigationCacheMode.Required;
                    try
                    {
                        ConnectedAnimation imageAnimation =
                            ConnectedAnimationService.GetForCurrentView().GetAnimation("UserImage");
                        if (imageAnimation != null)
                        {
                            ImageAnimationExists = true;
                            //imageAnimation.Completed += ImageAnimationClose_Completed;
                            //if (LatestGrid?.Name != InfoGrid.Name)
                            //    LatestGrid.Background = new SolidColorBrush(Helper.GetColorFromHex("#FF2E2E2E"));

                            imageAnimation.TryStart(UserImage);
                            //GridShadow.Visibility = GVSHOW.Visibility = Visibility.Collapsed;
                        }
                        else ImageAnimationExists = false;
                    }
                    catch { }
                }
                else if (e.Parameter is List<InstaUserShortFriendship> userShortFriendshipList)
                {
                    UserShortFriendship = userShortFriendshipList.FirstOrDefault();
                    UserShort = null;
                    Username = null;
                }
                //else if (e.Parameter is InstaUserShortFriendship userShortFriendship)
                //{
                //    UserShortFriendship = userShortFriendship;
                //    UserShort = null;
                //    Username = null;
                //}
                else if (e.Parameter is string username && !string.IsNullOrEmpty(username))
                {
                    Username = username;
                    UserShort = null;
                    UserShortFriendship = null;
                }
                else if (e.Parameter is object[] obj)
                {
                    //UserDetailsVM = new UserDetailsViewModel();
                    //DataContext = UserDetailsVM;
                    //NavigationCacheMode = NavigationCacheMode.Disabled;
                    //NavigationCacheMode = NavigationCacheMode.Required;
                    UserShort = obj[0] as InstaUserShort;
                    Username = null;
                    UserShortFriendship = null;
                    try
                    {
                        UserImage.Fill = obj[1] as Brush;
                        ConnectedAnimation imageAnimation =
                           ConnectedAnimationService.GetForCurrentView().GetAnimation("UserImage");
                        if (imageAnimation != null)
                        {
                            ImageAnimationExists = true;
                            //imageAnimation.Completed += ImageAnimationClose_Completed;
                            //if (LatestGrid?.Name != InfoGrid.Name)
                            //    LatestGrid.Background = new SolidColorBrush(Helper.GetColorFromHex("#FF2E2E2E"));

                            imageAnimation.TryStart(UserImage);
                            //GridShadow.Visibility = GVSHOW.Visibility = Visibility.Collapsed;
                        }
                        else
                            ImageAnimationExists = false;
                    }
                    catch { }
                }

                //if(e.NavigationMode == NavigationMode.New&& CanLoadFirstPopUp)
                //{

                //    LoadData();
                //}
            }
  
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            KeyDown -= OnKeyDownHandler;
            try
            {
                if (!ImageAnimationExists) return;
                var connectedAnimationService = ConnectedAnimationService.GetForCurrentView();
                connectedAnimationService.DefaultDuration = TimeSpan.FromMilliseconds(850);

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("UserPictureUserDetailsView",
                    UserImage);
            }
            catch { }
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
        //private void LVRefreshRequested(object sender, EventArgs e)
        //{
        //    UserDetailsVM.Refresh();
        //}




        private /*async*/ void RefreshControlRefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            // Respond to a request by performing a refresh and using the deferral object.
            using (var RefreshCompletionDeferral = args.GetDeferral())
            {
                UserDetailsVM.Refresh();
            }
        }

        private void RefreshControlRefreshStateChanged(Microsoft.UI.Xaml.Controls.RefreshVisualizer sender, Microsoft.UI.Xaml.Controls.RefreshStateChangedEventArgs args)
        {
            // Respond to visualizer state changes.
            // Disable the refresh button if the visualizer is refreshing.
            //("NewState::: " + args.NewState).PrintDebug();
            if (args.NewState == Microsoft.UI.Xaml.Controls.RefreshVisualizerState.Refreshing)
            {
                RefreshButton.IsEnabled = false;
            }
            else
            {
                RefreshButton.IsEnabled = true;
            }
        }







        private async void LVItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is InstaMedia media)
                {
                    var index = UserDetailsVM.MediaGeneratror.Items.IndexOf(media);

                    //Helpers.NavigationService.Navigate(typeof(Posts.ScrollableUserPostView), 
                    //    new object[] { UserDetailsVM.User, UserDetailsVM.MediaGeneratror , index});
                    ScrollableUserPostUc.Visibility = Visibility.Visible;
                    await Task.Delay(350);
                    ScrollableUserPostUc.ScrollTo(index);
                }

            }
            catch { }
        }

        private void LVContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            try
            {
                var elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
                if (args.InRecycleQueue)
                {
                    elementVisual.ImplicitAnimations = null;
                }
                else
                {
                    //Add implicit animation to each visual 
                    elementVisual.ImplicitAnimations = _elementImplicitAnimation;
                }
            }
            catch { }
        }
        #region Animation

        private CompositionAnimationGroup CreateOpacityAnimation()
        {
            ScalarKeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1.0f, 0.0f);
            fadeAnimation.Duration = TimeSpan.FromSeconds(.35);
            fadeAnimation.Target = "Opacity";
            //Add Animations to Animation group. 
            CompositionAnimationGroup animationGroup = _compositor.CreateAnimationGroup();
            animationGroup.Add(fadeAnimation);

            return animationGroup;
        }
        private CompositionAnimationGroup CreateOffsetAnimation()
        {

            //Define Offset Animation for the ANimation group
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromSeconds(.4);

            //Define Animation Target for this animation to animate using definition. 
            offsetAnimation.Target = "Offset";


            ScalarKeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, 0.8f);
            fadeAnimation.Duration = TimeSpan.FromSeconds(.4);
            fadeAnimation.Target = "Opacity";




            ////Define Rotation Animation for Animation Group. 
            //ScalarKeyFrameAnimation rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
            //rotationAnimation.InsertKeyFrame(.5f, 0.160f);
            //rotationAnimation.InsertKeyFrame(1f, 0f);
            //rotationAnimation.Duration = TimeSpan.FromSeconds(.4);

            ////Define Animation Target for this animation to animate using definition. 
            //rotationAnimation.Target = "RotationAngle";

            //Add Animations to Animation group. 
            CompositionAnimationGroup animationGroup = _compositor.CreateAnimationGroup();
            animationGroup.Add(offsetAnimation);
            //animationGroup.Add(rotationAnimation);
            animationGroup.Add(fadeAnimation);

            return animationGroup;
        }
        #endregion




















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
                    ToggleGoUpButtonAnimation(false);
                    ToggleRefreshButtonAnimation(false);
                }
                else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
                {
                    _isHideTitleGrid = false;
                    ToggleGoUpButtonAnimation(true);
                    ToggleRefreshButtonAnimation(true);
                }
                if (scrollViewer.VerticalOffset == 0)
                {
                    _isHideTitleGrid = true;
                    ToggleGoUpButtonAnimation(false);
                    ToggleRefreshButtonAnimation(false);
                }
                _lastVerticalOffset = scrollViewer.VerticalOffset;
            }
            catch { }
        }
        private void ToggleGoUpButtonAnimation(bool show)
        {
            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _goUpButtonVisual.CenterPoint = new Vector3((float)GoUpButton.ActualWidth / 2f, (float)GoUpButton.ActualHeight / 2f, 0f);
            _goUpButtonVisual.StartAnimation("Scale.X", scaleAnimation);
            _goUpButtonVisual.StartAnimation("Scale.Y", scaleAnimation);
        }
        private void ToggleRefreshButtonAnimation(bool show)
        {
            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _refreshButtonVisual.CenterPoint = new Vector3((float)GoUpButton.ActualWidth / 2f, (float)GoUpButton.ActualHeight / 2f, 0f);
            _refreshButtonVisual.StartAnimation("Scale.X", scaleAnimation);
            _refreshButtonVisual.StartAnimation("Scale.Y", scaleAnimation);
        }
        private void GoUpButtonClick(object sender, RoutedEventArgs e)
        {
            ScrollView.ScrollToElement(0);
            //LVStories.ScrollIntoView()
        }

        private void HyperlinkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UserDetailsVM.User.ExternalUrl.OpenUrl();
            }
            catch { }
        }

        private void LVHighlightsItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView != null)
                {
                    if (e.ClickedItem is InstaHighlightFeed reelFeed && reelFeed != null)
                    {
                        var index = LVHighlights.Items.IndexOf(reelFeed);
                        var list = new List<InstaReelFeed>();
                        foreach (var item in UserDetailsVM.Highlights)
                            list.Add(item.ToReelFeed());

                        Helpers.NavigationService.Navigate(typeof(Main.StoryView), new object[] { list, index });
                    }
                }
            }
            catch { }
        }

        private void UserImageTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {

                if (UserDetailsVM.Stories == null || UserDetailsVM.Stories != null && UserDetailsVM.Stories.Count == 0)
                    Helpers.NavigationService.Navigate(typeof(ImageVideoView), UserDetailsVM.User);
                else
                {
                    UserImageFlyout.Items.Clear();
                    var openPicture = new MenuFlyoutItem
                    {
                        Text = "Open profile picture"
                    };
                    openPicture.Click += MenuOpenPictureClick;

                    var openStory = new MenuFlyoutItem
                    {
                        Text = "Open stories"
                    };
                    openStory.Click += MenuOpenStoryClick;

                    UserImageFlyout.Items.Add(openPicture);
                    UserImageFlyout.Items.Add(openStory);
                    try
                    {
                        FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                    }
                    catch { }
                }

                //Helpers.NavigationService.Navigate(typeof(Main.StoryView), new object[] { UserDetailsVM.Stories.ToList(), 0 });
            }
            catch { }
        }

        private void MenuOpenPictureClick(object sender, RoutedEventArgs e)
        {
            Helpers.NavigationService.Navigate(typeof(ImageVideoView), UserDetailsVM.User);
        }
        private void MenuOpenStoryClick(object sender, RoutedEventArgs e)
        {
            Helpers.NavigationService.Navigate(typeof(Main.StoryView), new object[] { UserDetailsVM.Stories.ToList(), 0 });
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            //UserDetailsVM.Refresh();

            RefreshControl.RequestRefresh();
        }


        private void SeeAllButtonClick(object sender, RoutedEventArgs e)
        {

        }


        private async void FollowUnfollowButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn != null && btn.DataContext is InstaUserChaining user)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        if (user.FollowText == "Follow")
                        {
                            var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(user.Pk);
                            if (result.Succeeded)
                            {
                                if (result.Value.OutgoingRequest)
                                    user.FollowText = "Requested";
                                else if (result.Value.Following)
                                    user.FollowText = "Unfollow";
                            }
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{user.UserName}.\r\n" +
                                 $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        else if (user.FollowText == "Unfollow")
                        {
                            var result = await Helper.InstaApi.UserProcessor.UnFollowUserAsync(user.Pk);
                            if (result.Succeeded)
                                user.FollowText = "Follow";
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{user.UserName}.\r\n" +
                                 $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        //btn.DataContext = user;
                    });

                }
            }
            catch { }
        }

        private async void FollowUnFollowMainButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn != null)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        if (btn.Content.ToString() == "Follow")
                        {
                            var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(UserDetailsVM.User.Pk);
                            if (result.Succeeded)
                                UserDetailsVM.FriendshipStatus = result.Value?.ToStoryFriendshipStatus();
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{UserDetailsVM.User.UserName}.\r\n" +
                                         $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        else if (btn.Content.ToString() == "Unfollow" || btn.Content.ToString() == "Requested")
                        {
                            var result = await Helper.InstaApi.UserProcessor.UnFollowUserAsync(UserDetailsVM.User.Pk);
                            if (result.Succeeded)
                                UserDetailsVM.FriendshipStatus = result.Value?.ToStoryFriendshipStatus();
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{UserDetailsVM.User.UserName}.\r\n" +
                                    $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        else if (btn.Content.ToString() == "Unblock")
                        {
                            var result = await Helper.InstaApi.UserProcessor.UnBlockUserAsync(UserDetailsVM.User.Pk);
                            if (result.Succeeded)
                            {
                                UserDetailsVM.FriendshipStatus = result.Value?.ToStoryFriendshipStatus();
                                UserDetailsVM.Refresh();
                            }
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{UserDetailsVM.User.UserName}.\r\n" +
                                         $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                        else
                        if (btn.Content.ToString() == "Follow Back")
                        {
                            var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(UserDetailsVM.User.Pk);
                            if (result.Succeeded)
                                UserDetailsVM.FriendshipStatus = result.Value?.ToStoryFriendshipStatus();
                            else
                            {
                                if (result.Info.ResponseType == InstagramApiSharp.Classes.ResponseType.Spam)
                                    Helper.ShowNotify(ErrorMessages.FeedbackRequiredMessage, 2000);
                                else
                                    Helper.ShowNotify($"Error while sending follow request to '{UserDetailsVM.User.UserName}.\r\n" +
                                         $"Error message: {result.Info?.Message}", 2000);
                            }
                        }
                    });
                }
            }
            catch { }
        }

        private void FollowersButtonClick(object sender, RoutedEventArgs e)
        {
            if (UserDetailsVM.FriendshipStatus != null)
                if (UserDetailsVM.FriendshipStatus.IsPrivate && !UserDetailsVM.FriendshipStatus.Following)
                {
                    Helper.ShowNotify($"Since ${UserDetailsVM.User.UserName} is a private account, you are not allowed to see their followers/followings.\r\nFollow this user to see their followers/followings.", 2000);
                    return;
                }
            Helpers.NavigationService.Navigate(typeof(FollowView), new object[] { UserDetailsVM.User.ToUserShort(), 0 });
        }

        private void FollowingButtonClick(object sender, RoutedEventArgs e)
        {
            if (UserDetailsVM.FriendshipStatus != null)
                if (UserDetailsVM.FriendshipStatus.IsPrivate && !UserDetailsVM.FriendshipStatus.Following)
                {
                    Helper.ShowNotify($"Since ${UserDetailsVM.User.UserName} is a private account, you are not allowed to see their followers/followings.\r\nFollow this user to see their followers/followings.", 2000);
                    return;
                }
            Helpers.NavigationService.Navigate(typeof(FollowView), new object[] { UserDetailsVM.User.ToUserShort(), 1 });
        }

        private void CopyBiographyMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UserDetailsVM.User.Biography.CopyText();
                Helper.ShowNotify($"{UserDetailsVM.User.UserName}'s biography copied ;)");
            }
            catch { }
        }

        private void BiographyGridTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch { }
        }

        private void CopyUsernameMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UserDetailsVM.User.UserName.CopyText();
                Helper.ShowNotify($"{UserDetailsVM.User.UserName} copied ;)");
            }
            catch { }
        }
        private void CopyUsernameAddressMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var urlAddress = Helper.InstagramUrl + UserDetailsVM.User.UserName;
                urlAddress.CopyText();
                Helper.ShowNotify($"{urlAddress} copied ;)");
            }
            catch { }
        }
        private void UsernameGridTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch { }
        }

        private void CopyExternalUrlMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UserDetailsVM.User.ExternalUrl.CopyText();
                Helper.ShowNotify("Url copied ;)");
            }
            catch { }
        }

        private void ChainingItemTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid grid && grid != null && grid.DataContext is InstaUserChaining user)
                    Helper.OpenProfile(user.ToUserShortFriendship()/*.ToListExt()*/);
            } 
            catch { }
        }

        private async void MoreButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await new ContentDialogs.UserProfileMenusDialog(UserDetailsVM.User, UserDetailsVM.FriendshipStatus).ShowAsync();
            }
            catch { }
        }


        #region LOADINGS
        public void ShowTopLoading() => TopLoading.Start();
        public void HideTopLoading() => TopLoading.Stop();


        public void ShowBottomLoading() => BottomLoading.Start();

        public void HideBottomLoading() => BottomLoading.Stop();
        #endregion LOADINGS

    }
}
