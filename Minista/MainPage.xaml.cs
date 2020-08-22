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

using Minista.Helpers;
using static Helper;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.Media.Editing;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using UICompositionAnimations.Enums;
using Windows.UI.Popups;
using Windows.Graphics.Imaging;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Background;
using Thrift.Collections;
using Windows.UI.Notifications.Management;
using InstagramApiSharp.API.RealTime;

namespace Minista
{
    public sealed partial class MainPage : Page
    {
        public static string NavigationUriProtocol;
        public static MainPage Current;
        public MediaElement ME => mediaElement;

        Windows.System.Display.DisplayRequest ScreenOnRequest;
        public RealtimeClient RealtimeClient { get;  set; }

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            Loaded += MainPageLoaded;
            Window.Current.CoreWindow.KeyDown += OnKeyboards;
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
            //KeyDown += MainPageKeyDown;
            CreateConfig();
            try
            {
                if (!CheckLogin())
                {
                    AllowDrop = true;
                    try
                    {
                        DragOver -= OnDragOver;
                        Drop -= OnDrop;
                    }
                    catch { }
                    try
                    {
                        DragOver += OnDragOver;
                        Drop += OnDrop;
                    }
                    catch { }
                }
            }
            catch { }
        }



        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Drop here to upload";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
        }
        async private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                try
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    if (items.Count > 0)
                    {
                        if (items[0] is StorageFile file)
                        {
                            if (file.Path.IsSupportedImage()) // IsSupportedVideo ?
                            {
                                if (NavigationService.Frame.Content is Views.Direct.ThreadView thread)
                                {
                                    thread.UploadFile(file);
                                }
                                else
                                    await new ContentDialogs.FileAssociationDialog(file).ShowAsync();
                            }
                            else
                                ShowNotify("This file is not supported.\r\n" + file.Path, 3000);
                        }
                    }
                }
                catch { }
            }
        }

        private void CoreWindow_SizeChanged(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
        {
            //(args.Size.Width + "x" + args.Size.Height).PrintDebug();
        }

        //private void MainPageKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    try
        //    {
        //        ("[OriginalKey]\tKey Pressed: " + e.OriginalKey + "\t\t" + (int)e.OriginalKey).PrintDebug();
        //        "".PrintDebug();
        //        "".PrintDebug();
        //    }
        //    catch { }
        //}

        private async void OnKeyboards(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            try
            {
                CoreVirtualKeyStates controlKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                var isCtrlDown = (controlKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

                CoreVirtualKeyStates shiftKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
                var isShiftDown = (shiftKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                var shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
                //var alt = Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated;
                //("[VirtualKey]\tKey Pressed: " + args.VirtualKey + "\t\t" + (int)args.VirtualKey).PrintDebug();
                switch (args.VirtualKey)
                {
                    case VirtualKey.Escape:
                        //NavigationService.GoBack();
                        break;
                }
                if (isCtrlDown && args.VirtualKey == VirtualKey.Left || args.VirtualKey == VirtualKey.Escape)
                    NavigationService.GoBack();

                if (isCtrlDown && args.VirtualKey == VirtualKey.V || isShiftDown && args.VirtualKey == VirtualKey.Insert)
                {
                    if (InstaApi != null && InstaApi.IsUserAuthenticated)
                    {
                        DataPackageView dataPackageView = Clipboard.GetContent();
                        if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                        {
                            var items = await dataPackageView.GetStorageItemsAsync();
                            if (items.Count > 0)
                                if (items[0] is StorageFile file)
                                    if (file.Path.IsSupportedImage()) // IsSupportedVideo ?
                                    {
                                        if (NavigationService.Frame.Content is Views.Direct.ThreadView thread)
                                        {
                                            thread.UploadFile(file);
                                        }
                                        else
                                            await new ContentDialogs.FileAssociationDialog(file).ShowAsync();
                                    }
                                    else
                                        ShowNotify("This file is not supported.\r\n" + file.Path, 3000);
                        }
                        else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
                        {
                            var bitmap = await dataPackageView.GetBitmapAsync();
                            var decoder = await BitmapDecoder.CreateAsync(await bitmap.OpenReadAsync());
                            var file = await GenerateRandomOutputFile();
                            var encoder = await BitmapEncoder.CreateForTranscodingAsync(await file.OpenAsync(FileAccessMode.ReadWrite), decoder);
                            await encoder.FlushAsync();
                            if (NavigationService.Frame.Content is Views.Direct.ThreadView thread)
                            {
                                thread.UploadFile(file);
                            }
                            else
                                await new ContentDialogs.FileAssociationDialog(file).ShowAsync();
                        }
                    }
                }
            }
            catch { }
        }

        public void HideHeaders()
        {
            GridFrame.Margin = new Thickness(0);
            SplitViewPaneGrid.Margin = new Thickness(0);
            StackPanelTitle.Visibility = Visibility.Collapsed;
        }
        public void ShowHeaders()
        {
            if(NavigationService.Frame.Content is Views.Sign.SignInView)
            {
                HideHeaders();
                return;
            }
            if (SettingsHelper.Settings.HeaderPosition == Classes.HeaderPosition.Top)
            {
                GridFrame.Margin = new Thickness(0, 52, 0, 0);
                SplitViewPaneGrid.Margin = new Thickness(0, 52, 0, 0);
                AppTitleBarORG.Height = 52;
                AppTitleBar.VerticalAlignment = VerticalAlignment.Top;
                if (DeviceUtil.IsDesktop)
                {
                    AppTitleBar.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                    AppTitleBar.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                if (DeviceUtil.IsMobile)
                {
                    GridFrame.Margin = new Thickness(0, 0, 0, 52);
                    SplitViewPaneGrid.Margin = new Thickness(0, 0, 0, 52);
                    AppTitleBarORG.Height = 0;
                }
                else
                { 
                    GridFrame.Margin = new Thickness(0, 42, 0, 52);
                    SplitViewPaneGrid.Margin = new Thickness(0, 42, 0, 52);
                    AppTitleBarORG.Height = 42;
                }
                AppTitleBar.VerticalAlignment = VerticalAlignment.Bottom;
                AppTitleBar.HorizontalAlignment = HorizontalAlignment.Center;
            }
            SetTitleBarLayout();
            //AppTitleBarORG
            if(NavigationService.Frame.Content is Views.Sign.SignInView)
                StackPanelTitle.Visibility = Visibility.Visible;
            else
            StackPanelTitle.Visibility = Visibility.Visible;
        }

        public void SetStackPanelTitleVisibility(Visibility visibility = Visibility.Collapsed)
        {
            StackPanelTitle.Visibility = visibility;
        }
        public void NavigateToMainView(bool flag = false)
        {
            SetStackPanelTitleVisibility(Visibility.Visible);
            ShowHeaders();
            if (!string.IsNullOrEmpty(NavigationUriProtocol))
            {
                HandleUriProtocol();
                return;
            }
            try
            {
                if (flag)
                {
                    if (Views.Main.MainView.Current != null)
                    {
                        Views.Main.MainView.Current.ResetPageCache();
                    }
                }
            }
            catch { }
            NavigationService.Navigate(typeof(Views.Main.MainView));
            try
            {
                if (flag)
                {
                    NavigateToMainViewAsync();
                }
            }
            catch { }
        }
        async void NavigateToMainViewAsync()
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, /*async*/ () =>
                {
                    if (Views.Direct.InboxView.Current != null)
                    {
                        Views.Direct.InboxView.Current.ResetPageCache();
                        //ViewModels.Direct.InboxViewModel.ResetInstance();
                    }
                    UserHelper.GetSelfUser();
                    UserHelper.GetBanyan();

                    if (Views.Direct.ThreadView.Current != null)
                    {
                        Views.Direct.ThreadView.Current.ResetPageCache();
                    }
                    if (Views.Direct.DirectRequestsThreadView.Current != null)
                    {
                        Views.Direct.DirectRequestsThreadView.Current.ResetPageCache();
                    }
                    if (Views.Direct.DirectRequestsView.Current != null)
                    {
                        Views.Direct.DirectRequestsView.Current.ResetPageCache();
                    }
                    if (Views.Main.ActivitiesView.Current != null)
                    {
                        Views.Main.ActivitiesView.Current.ResetPageCache();
                        //Views.Main.ActivitiesView.Current.NavigationCacheMode = NavigationCacheMode.Disabled;
                        //Views.Main.ActivitiesView.Current.NavigationCacheMode = NavigationCacheMode.Enabled;
                    }
                    if (Views.Main.ExploreView.Current != null)
                    {
                        Views.Main.ExploreView.Current.ResetPageCache();
                        //Views.Main.ExploreView.Current.NavigationCacheMode = NavigationCacheMode.Disabled;
                        //Views.Main.ExploreView.Current.NavigationCacheMode = NavigationCacheMode.Enabled;
                    }
                    if (Views.Main.ExploreView.Current != null)
                    {
                        Views.Main.ExploreView.Current.ResetPageCache();
                    }
                    if (Views.Main.ExploreClusterView.Current != null)
                    {
                        Views.Main.ExploreClusterView.Current.ResetPageCache();
                    }
                    if (Views.Main.LikersView.Current != null)
                    {
                        Views.Main.LikersView.Current.ResetPageCache();
                    }
                    if (Views.Searches.SearchView.Current != null)
                    {
                        Views.Searches.SearchView.Current.ResetPageCache();
                        //Views.Searches.SearchView.Current.NavigationCacheMode = NavigationCacheMode.Disabled;
                        //Views.Searches.SearchView.Current.NavigationCacheMode = NavigationCacheMode.Enabled;
                    }
                    if (Views.Infos.SavedPostsView.Current != null)
                    {
                        Views.Infos.SavedPostsView.Current.ResetPageCache();
                    }
                    if (Views.Infos.CloseFriendsView.Current != null)
                    {
                        Views.Infos.CloseFriendsView.Current.ResetPageCache();
                    }
                    if (Views.Infos.ArchiveView.Current != null)
                    {
                        Views.Infos.ArchiveView.Current.ResetPageCache();
                    }
                    if (Views.Infos.ProfileDetailsView.Current != null)
                    {
                        Views.Infos.ProfileDetailsView.Current.ResetPageCache();
                        //Views.Searches.SearchView.Current.NavigationCacheMode = NavigationCacheMode.Disabled;
                        //Views.Searches.SearchView.Current.NavigationCacheMode = NavigationCacheMode.Enabled;
                    }
                    if (Views.Infos.UserDetailsView.Current != null)
                    {
                        Views.Infos.UserDetailsView.Current.ResetPageCache();
                    }
                    if (Views.Infos.FollowRequestsView.Current != null)
                    {
                        Views.Infos.FollowRequestsView.Current.ResetPageCache();
                        //Views.Main.ActivitiesView.Current.NavigationCacheMode = NavigationCacheMode.Disabled;
                        //Views.Main.ActivitiesView.Current.NavigationCacheMode = NavigationCacheMode.Enabled;
                    }
                    if (Views.Infos.FollowView.Current != null)
                    {
                        Views.Infos.FollowView.Current.ResetPageCache();
                    }
                    if (Views.Infos.HashtagView.Current != null)
                    {
                        Views.Infos.HashtagView.Current.ResetPageCache();
                    }
                    if (Views.Infos.RecentFollowersView.Current != null)
                    {
                        Views.Infos.RecentFollowersView.Current.ResetPageCache();
                    }

                    if (Views.Posts.ScrollableUserPostView.Current != null)
                    {
                        Views.Posts.ScrollableUserPostView.Current.ResetPageCache();
                    }
                    if (Views.Posts.MultiplePostView.Current != null)
                    {
                        Views.Posts.MultiplePostView.Current.ResetPageCache();
                    }
                    if (Views.Posts.SinglePostView.Current != null)
                    {
                        Views.Posts.SinglePostView.Current.ResetPageCache();
                    }
                    if (Views.Posts.CommentView.Current != null)
                    {
                        Views.Posts.CommentView.Current.ResetPageCache();
                    }
                    if (Views.Posts.ScrollableExplorePostView.Current != null)
                    {
                        Views.Posts.ScrollableExplorePostView.Current.ResetPageCache();
                    }
                    if (Views.TV.TVView.Current != null)
                    {
                        Views.TV.TVView.Current.ResetPageCache();
                    }
                    try
                    {
                        if (RealtimeClient != null)
                        {
                            try
                            {
                                RealtimeClient.Shutdown();
                            }
                            catch { }
                        }
                        RealtimeClient = new RealtimeClient(InstaApi);
                    }
                    catch { }
                });
            }
            catch { }
        }

        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            CreateConfig();


            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            //AppTitleBar.Height = coreTitleBar.Height;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBarLayoutMetricsChanged;
            // Set XAML element as a draggable region.
            //AppTitleBar.Height = coreTitleBar.Height;

            UpdateTitleBarLayout(coreTitleBar);
            Window.Current.SetTitleBar(AppTitleBarORG);
            if (InstaApi == null || InstaApi != null && !InstaApi.IsUserAuthenticated)
            {
                SetStackPanelTitleVisibility(Visibility.Collapsed);
                NavigationService.Navigate(typeof(Views.Sign.SignInView));
            }
            else
                NavigateToMainView();

            if (!SettingsHelper.Settings.AskedAboutPosition)
            {
                SettingsGrid.Visibility = Visibility.Visible;
            }
            if (ScreenOnRequest != null)
                ScreenOnRequest.RequestActive();
            CheckLicense();
            try
            {
                if (Passcode.IsEnabled)
                {
                    PassCodeView.Visibility = Visibility.Visible;
                    LockControl.Visibility = Visibility.Visible;
                }
                else
                {
                    PassCodeView.Visibility = Visibility.Collapsed;
                    LockControl.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch { }
            try
            {
                UserNotificationListener listener = UserNotificationListener.Current;
                await listener.RequestAccessAsync();
            }
            catch { }
        }

        private void CoreTitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }
        private double SystemOverlayLeftInset = 0;
        private double SystemOverlayRightInset = 0;

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            try
            {
                SystemOverlayLeftInset = coreTitleBar.SystemOverlayLeftInset;
                SystemOverlayRightInset = coreTitleBar.SystemOverlayRightInset;
                SetTitleBarLayout();
                // Update title bar control size as needed to account for system size changes.
                //AppTitleBar.Height = coreTitleBar.Height;
            }
            catch { }
        }
        void SetTitleBarLayout()
        {
            try
            {
                if (SettingsHelper.Settings.HeaderPosition == Classes.HeaderPosition.Top)
                {
                    // Get the size of the caption controls area and back button 
                    // (returned in logical pixels), and move your content around as necessary.
                    LeftPaddingColumn.Width = new GridLength(SystemOverlayLeftInset);
                    RightPaddingColumn.Width = new GridLength(SystemOverlayRightInset);
                }
                else
                {
                    LeftPaddingColumn.Width = new GridLength(0);
                    RightPaddingColumn.Width = new GridLength(0);
                }
            }
            catch { }
        }
        async void CheckLicense()
        {
            try
            {
                await GetLicenseState();
            }
            catch { }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //try
            //{
            //    Application.Current.Resources["DefaultBackgroundColor"] = Helper.GetColorBrush("#18227c");
            //}
            //catch (Exception ex)
            //{
            //}

            try
            {
                if (DeviceUtil.IsXbox)
                    FullscreenModeInXbox();
            }
            catch { }
            ChangeTileBarTheme();
            CreateCachedFilesFolder();
            NavigationService.StartService();
            ScreenOnRequest = new Windows.System.Display.DisplayRequest();
            if (!DeviceHelper.IsThisMinista())
            {
                await new MessageDialog("Oops, It seems you changed my app package to crack it.\r\n" +
                    "Well done, now use my app, if you can:d\r\n" +
                    "Pay the price dude, don't be cheap:d\r\n" +
                    "Bye bye").ShowAsync();
                try
                {
                    CoreApplication.Exit();
                }
                catch
                {
                    try
                    {
                        Application.Current.Exit();
                    }
                    catch { }
                }
                return;
            }
            try
            {
                var flag = await GetLicenseState();
                if (!flag)
                    PurchaseMessage();
            }
            catch { }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.StopService();
            try
            {
                if (ScreenOnRequest != null)
                    ScreenOnRequest.RequestRelease();
            }
            catch { }
            try
            {
                DragOver -= OnDragOver;
                Drop -= OnDrop;
            }
            catch { }
        }
        public void ShowInAppNotify(string text, int duration = 1800)
        {
            try
            {
                InAppNotify.Show(text, duration);

                //// Show notification using a DataTemplate
                //object inAppNotificationWithButtonsTemplate;
                //bool isTemplatePresent = Resources.TryGetValue("InAppNotificationWithButtonsTemplate", out inAppNotificationWithButtonsTemplate);

                //if (isTemplatePresent && inAppNotificationWithButtonsTemplate is DataTemplate)
                //{
                //    InAppNotify.Show(inAppNotificationWithButtonsTemplate as DataTemplate);
                //}
            }
            catch { }
        }

        public void ShowLoading(string text = null)
        {
            if (text == null)
                text = string.Empty;
            LoadingText.Text = text;
            LoadingPb.IsActive = true;
            LoadingGrid.Visibility = Visibility.Visible;
        }

        public void HideLoading()
        {
            LoadingPb.IsActive = false;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        public void SetUserAndPicture()
        {
            if (InstaApi == null) return;
            //if (InstaApi.IsUserAuthenticated)
            //{
            //    var user = InstaApi.GetLoggedUser().LoggedInUser;
            //    UserPicture.Fill = user.ProfilePicture.GetImageBrush();
            //    UserNameText.Text = user.UserName;
            //    SplitView.CompactPaneLength = 60;
            //    SplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
            //    //SplitView.IsPaneOpen = true;
            //}
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            //SearchButton.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(typeof(Views.Searches.SearchView));
        }
         
        private void DirectButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Direct.InboxView));
        }
        //MinistaHelper.Push.PushClient PushClient;
        private void ActivityButtonClick(object sender, RoutedEventArgs e)
        {
            //PushClient = new MinistaHelper.Push.PushClient(InstaApi, false);
            //PushClient.MessageReceived += PushClient_MessageReceived;

            //PushClient.Start();
            NavigationService.Navigate(typeof(Views.Main.ActivitiesView));
        }

        private void PushClient_MessageReceived(object sender,InstagramApiSharp.API. PushReceivedEventArgs e)
        {
            PushHelper.HandleNotify(e.NotificationContent, InstaApiList);
        }

        private  void ProfileButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Infos.ProfileDetailsView));
        }
        //private void PushClientMessageReceived(object sender, InstagramApiSharp.API.Push.MessageReceivedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine(e?.Json);
        //    Helpers.PushHelper.HandleNotify(e.NotificationContent, Helper.InstaApiList);
        //    //Helpers.NotificationHelper.ShowToast(e.NotificationContent.Message, e.NotificationContent.OptionalAvatarUrl, e.NotificationContent.Title ?? "");
        //}

        private void ExploreButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Main.ExploreView));
        }




        public void SetDirectMessageCount(InstaDirectInboxContainer inbox)
        {
            try
            {
                if (inbox == null)
                {
                    DirectMessageCountGrid.Visibility = Visibility.Collapsed;
                    return;
                }

                if(inbox.Inbox.UnseenCount > 0)
                {
                    DirectMessageCountText.Text = inbox.Inbox.UnseenCount.ToString();
                    DirectMessageCountGrid.Visibility = Visibility.Visible;
                }
                else
                    DirectMessageCountGrid.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        public void LoggedOut() // logout is completed?
        {
            var switchToAnotherAccount = false;
            if (InstaApiList.Count > 0)
            {
                switchToAnotherAccount = true;
                InstaApiList.Remove(InstaApi);
                InstaApi = null;
                InstaApi = InstaApiList[0];
            }

            if (!switchToAnotherAccount)
            {
                SetStackPanelTitleVisibility(Visibility.Collapsed);
                NavigationService.Navigate(typeof(Views.Sign.SignInView));
            
            }
            else
            {
                InstaApiSelectedUsername = InstaApi.GetLoggedUser().UserName.ToLower();
                SettingsHelper.SaveSettings();
                try
                {
                    NavigationService.HideBackButton();
                }
                catch { }
                UserChanged = true;
                try
                {
                    Current.NavigateToMainView(true);
                }
                catch { }
            }
            "You've Been Logged Out.".ShowMsg();
            NavigationService.RemoveAllBackStack();
        }

        private void HomeButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Main.MainView));
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void ShowBackButton() => BackButton.Visibility = Visibility.Visible;
        public void HideBackButton() => BackButton.Visibility = Visibility.Collapsed;

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BottomRadio.IsChecked == true)
                {
                    SettingsHelper.Settings.HeaderPosition = Classes.HeaderPosition.Bottom;
                    SettingsHelper.Settings.AskedAboutPosition = true;
                    SettingsHelper.SaveSettings();
                }
                else
                {
                    SettingsHelper.Settings.HeaderPosition = Classes.HeaderPosition.Top;
                    SettingsHelper.Settings.AskedAboutPosition = true;
                    SettingsHelper.SaveSettings();
                }
                ShowHeaders();
            }
            catch { }

            SettingsGrid.Visibility = Visibility.Collapsed;
        }

        //private async void AddUserButtonClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        await new ContentDialogs.AddOrChooseUserDialog().ShowAsync();
        //    }
        //    catch { }
        //}
        public void ShowMediaUploadingUc()
        {
            try
            {
                MediaUploadingUc.Visibility = Visibility.Visible;
                MediaUploadingUc.Start();
            }
            catch { }
        }
        public void HideMediaUploadingUc()
        {
            try
            {
                MediaUploadingUc.Visibility = Visibility.Collapsed;
                MediaUploadingUc.Stop();
            }
            catch { }
        }
        //PhotoUploaderHelper Uploader = new PhotoUploaderHelper();
        //PhotoAlbumUploader AlbumUploader = new PhotoAlbumUploader();
        private /*async*/ void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(typeof(Views.Uploads.UploadView));
            //NavigationService.Navigate(typeof(Views.Posts.UploadPostView));
            //NavigationService.Navigate(typeof(Views.Posts.UploadStoryView));
            //return;
            //FileOpenPicker openPicker = new FileOpenPicker
            //{
            //    ViewMode = PickerViewMode.Thumbnail,
            //    SuggestedStartLocation = PickerLocationId.PicturesLibrary
            //};
            //openPicker.FileTypeFilter.Add(".jpg");
            //openPicker.FileTypeFilter.Add(".bmp");
            ////openPicker.FileTypeFilter.Add(".gif");
            //openPicker.FileTypeFilter.Add(".png");
            //var files = await openPicker.PickMultipleFilesAsync();
            //if (files != null && files.Count > 0)
            //{
            //    if(files.Count == 1)
            //    {
            //        using (var photo = new PhotoHelper())
            //        {
            //            var fileToUpload = await photo.SaveToImageForPost(files[0]);
            //            Random rnd = new Random();
            //            Uploader.UploadSinglePhoto(fileToUpload, "TEEEEEEEEEEST\r\n\r\n\r\n" + DateTime.Now.ToString(), null);
            //        }
            //    }
            //    else
            //    {
            //        List<StorageFile> list = new List<StorageFile>();
            //        foreach (var f in files)
            //        {
            //            using (var photo = new PhotoHelper())
            //            {
            //                var fileToUpload = await photo.SaveToImageForPost(f);
            //                list.Add(fileToUpload);
            //            }
            //        }
            //        AlbumUploader.SetFiles(list.ToArray(), $"ALBUM UPPPPP\r\n\r\n\r\n{DateTime.Now}\r\n\r\n\r\n" +
            //            $"RMT");
            //    }
            //}
        }

        private void UploadNewStoryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Focus(FocusState.Pointer);
            }
            catch { }
            NavigationService.Navigate(typeof(Views.Uploads.UploadStoryView));
        }
        private void UploadStoryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Focus(FocusState.Pointer);
            }
            catch { }
            NavigationService.Navigate(typeof(Views.Posts.UploadStoryView));
        }
        private void UploadPostButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Focus(FocusState.Pointer);
            }
            catch { }
            NavigationService.Navigate(typeof(Views.Uploads.UploadView));
        }
        private void UploadOldPostButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Focus(FocusState.Pointer);
            }
            catch { }

            NavigationService.Navigate(typeof(Views.Posts.UploadPostView));
        }
        public void HandleUriFile(FileActivatedEventArgs e)
        {
            try
            {
                if (e.Files.Any())
                {
                    if (e.Files[0] is StorageFile file)
                        HandleUriFile(file);
                } 
            }
            catch {}
        }
        public async void HandleUriFile(StorageFile file)
        {
            try
            {
                await new ContentDialogs.FileAssociationDialog(file).ShowAsync();
            }
            catch { }
        }
        public void HandleUriProtocol()
        {
            if (string.IsNullOrEmpty(NavigationUriProtocol)) return;
            var uri = NavigationUriProtocol;
            UriHelper.HandleUri(uri);
            NavigationUriProtocol = null;
        }
        public void NavigateToUrl(string urlProtocol)
        {
            try
            {
                if (string.IsNullOrEmpty(urlProtocol)) return;
                var url = urlProtocol;
                if (url.ToLower().Contains("instagram.com/"))
                {
                    var n = url.Substring(url.IndexOf("instagram.com/") + "instagram.com/".Length);

                    OpenProfile(n);
                }
                else if (url.ToLower().Contains("ramtinak@live"))
                    ($"mailto:{url}").OpenUrl();
                else
                    url.OpenUrl();
            }
            catch { }
        }
        

        public void ShowActitivityNotify(InstaActivityCount count)
        {
            try
            {
                if (ActivityNotifyUc == null) return;

                var pos = ActivityButton.GetPosition();

                double x = pos.X + 5;
                double y = 0;
                if (AppTitleBar.VerticalAlignment == VerticalAlignment.Top)
                     y = pos.Y + ActivityButton.ActualHeight - 14;
                else
                {
                    if (!DeviceUtil.IsMobile)
                        y = pos.Y - ActivityButton.ActualHeight + 18;
                    else
                        y = pos.Y - ActivityButton.ActualHeight ;
                }
                var transform = new CompositeTransform
                {
                    TranslateX = x,
                    TranslateY = y
                };

                ActivityNotifyUc.Show(transform, count, AppTitleBar.VerticalAlignment == VerticalAlignment.Bottom);
         
            }
            catch { }
        }

        //private void SettingsButtonClick(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(typeof(Views.Settings.SettingsView));
        //}
    }
}
