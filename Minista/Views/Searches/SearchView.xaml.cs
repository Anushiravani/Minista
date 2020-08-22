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
using InstagramApiSharp.Classes.Models;
using Windows.UI.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using System.Threading.Tasks;
using System.Numerics;

namespace Minista.Views.Searches
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchView : Page
    {
        readonly Compositor _compositor;

        Visual _baseGridVisual;
        bool First = true;
        ScrollViewer ScrollView, ScrollView2, ScrollView3, ScrollView4;
        public static SearchView Current;
        public SearchView()
        {
            this.InitializeComponent();
            Current = this;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            Loaded += SearchViewLoaded;
        }

        private void SearchViewLoaded(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                SearchVM.RunLoadMore();
                First = false;
            }
            SetLVs();
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
                {
                    if (SearchText.Text.Length <= 2)
                        SearchVM.RunLoadMore();
                    else
                        DoSearch();
                }
            }
            catch { }
        }
        void SetLVs()
        {
            try
            {
                SearchVM.SetHashtagScrollViewer(TagsLV);
                SearchVM.SetPlaceScrollViewer(PlacesLV);
            }
            catch { }
        }
        private void UserOrHashtagTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid gid && gid.DataContext is InstaDiscoverRecentSearchesItem item && item != null)
                {
                    if(item.IsHashtag)
                        "HashtagTapped".PrintDebug();
                    else
                        Helper.OpenProfile(item.User.ToUserShort());
                }
            }
            catch { }
        }



        private async void DismissUserClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarButton btn && btn.DataContext is InstaDiscoverRecentSearchesItem item && item != null)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        SearchVM.RemoveItem(item.User.Pk);
                        var dismiss = await Helper.InstaApi.DiscoverProcessor.HideSearchEntityAsync(item.User.Pk);
                    });
                }
            }
            catch { }
        }

        private void SearchTextTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (SearchText.Text.Length <= 2)
                {
                    SearchVM.AccountSearches.Clear();
                    SearchVM.AccountSearches.AddRange(SearchVM.AccountSearchesPrivate);
                }
                else
                    DoSearch();
            }
            catch { }
        }
        private void SearchTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    DoSearch();
            }
            catch { }
        }
        void DoSearch()
        {
            try
            {
                switch (MainPivot.SelectedIndex)
                {
                    case 0:
                    case 1:// accounts
                        SearchVM.SearchAccounts(SearchText.Text);
                        break;
                    case 2:// hashtags
                        SearchVM.SearchTags(SearchText.Text);
                        break;
                    case 3:// places
                        SearchVM.SearchPlaces(SearchText.Text);
                        break;

                }
            }
            catch { }
        }
        private void UserTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid gid && gid.DataContext is InstaUser item && item != null)
                {
                    Helper.OpenProfile(item.ToUserShort());
                }
            }
            catch { }
        }
        private void SearchButtonClick(object sender, RoutedEventArgs e) => DoSearch();

        private void HashtagTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid gid && gid.DataContext is InstaHashtag item && item != null)
                {
                    "HashtagTapped".PrintDebug();
                }
            }
            catch { }
        }

        private void ItemsLVLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ScrollView = ItemsLV.FindScrollViewer();
                if (ScrollView != null)
                    ScrollView.ViewChanging += ScrollViewViewChanging;
            }
            catch { }
        }

        private void AccountsLVLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ScrollView2 = AccountsLV.FindScrollViewer();
                if (ScrollView2 != null)
                    ScrollView2.ViewChanging += ScrollViewViewChanging;
            }
            catch { }
        }

        private void TagsLVLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ScrollView3 = TagsLV.FindScrollViewer();
                if (ScrollView3 != null)
                    ScrollView3.ViewChanging += ScrollViewViewChanging;
            }
            catch { }
        }

        private void PlacesLVLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ScrollView4 = PlacesLV.FindScrollViewer();
                if (ScrollView4 != null)
                    ScrollView4.ViewChanging += ScrollViewViewChanging;
            }
            catch { }
        }

        private void PlaceTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid gid && gid.DataContext is InstaPlaceShort/*InstaLocationShort*/ item && item != null)
                {
                    "PlaceTapped".PrintDebug();
                }
            }
            catch { }
        }

        private void MainPivotSelectionXChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //SetLVs();
            }
            catch { }
        }
        private async void FollowUnFollowMainButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn != null&& btn.DataContext is InstaUser user)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (btn.Content.ToString() == "Follow")
                        {
                            var result = await Helper.InstaApi.UserProcessor.FollowUserAsync(user.Pk);
                            if (result.Succeeded)
                                user.FriendshipStatus = result.Value?.ToFriendshipShortStatus();
                            else
                                Helper.ShowNotify($"Error while sending follow request to '{user.UserName}.\r\n" +
                                    $"Error message: {result.Info?.Message}", 2000);
                        }
                        else if (btn.Content.ToString() == "Unfollow" || btn.Content.ToString() == "Requested")
                        {
                            var result = await Helper.InstaApi.UserProcessor.UnFollowUserAsync(user.Pk);
                            if (result.Succeeded)
                                user.FriendshipStatus = result.Value?.ToFriendshipShortStatus();
                            else
                                Helper.ShowNotify($"Error while sending follow request to '{user.UserName}.\r\n" +
                                    $"Error message: {result.Info?.Message}", 2000);
                        }
                    });
                }
            }
            catch { }
        }



        private void TagsLVItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if(e.ClickedItem != null && e.ClickedItem is InstaHashtag hashtag)
                    Helpers.NavigationService.Navigate(typeof(Infos.HashtagView), hashtag.Name);
            }
            catch { }
        }




        Grid BaseGrid, GridHeader;

        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            BaseGrid = sender as Grid;
            _baseGridVisual = BaseGrid.GetVisual();
            try
            {
                if (GridHeader != null && BaseGrid != null)
                    GridHeader.Height = BaseGrid.ActualHeight;
            }
            catch { }
        }


        private void GridHeaderLoaded(object sender, RoutedEventArgs e)
        {
            GridHeader = sender as Grid;
            try
            {
                if (GridHeader != null && BaseGrid != null)
                    GridHeader.Height = BaseGrid.ActualHeight;
            }
            catch { }
        }

        private void GridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if(GridHeader != null && BaseGrid != null)
                GridHeader.Height = BaseGrid.ActualHeight;
            }
            catch { }
        }

        //private double _lastVerticalOffset;


        //private bool _isHideTitleGrid;




        private void ScrollViewViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //try
            //{
            //    var scrollViewer = sender as ScrollViewer;
            //    if ((scrollViewer.VerticalOffset - _lastVerticalOffset) > 5 && !_isHideTitleGrid)
            //    {
            //        _isHideTitleGrid = true;
            //        ToggleGoUpButtonAnimation(false);
            //    }
            //    else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
            //    {
            //        _isHideTitleGrid = false;
            //        ToggleGoUpButtonAnimation(true);
            //    }
            //    _lastVerticalOffset = scrollViewer.VerticalOffset;
            //}
            //catch { }
        }

        private async void ToggleGoUpButtonAnimation(bool show)
        {
            if (_baseGridVisual == null)
                return;
            if (show)
                BaseGrid.Visibility = Visibility.Visible;

            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _baseGridVisual.CenterPoint = new Vector3((float)BaseGrid.ActualWidth / 2f, (float)BaseGrid.ActualHeight / 2f, 0f);
            _baseGridVisual.StartAnimation("Scale.X", scaleAnimation);
            _baseGridVisual.StartAnimation("Scale.Y", scaleAnimation);
            try
            {
                if (!show)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await Task.Delay(510);
                        BaseGrid.Visibility = Visibility.Collapsed;
                    });

                }
            }
            catch { }
        }

        #region LOADINGS Top
        public void ShowTopLoadingTop() => TopLoadingTop.Start();
        public void HideTopLoadingTop() => TopLoadingTop.Stop();
        #endregion LOADINGS Top



        #region LOADINGS Account
        public void ShowTopLoadingAccount() => TopLoadingAccount.Start();
        public void HideTopLoadingAccount() => TopLoadingAccount.Stop();
        #endregion LOADINGS Account



        #region LOADINGS Tag
        public void ShowTopLoadingTag() => TopLoadingTag.Start();
        public void HideTopLoadingTag() => TopLoadingTag.Stop();


        public void ShowBottomLoadingTag() => BottomLoadingTag.Start();
        public void HideBottomLoadingTag() => BottomLoadingTag.Stop();
        #endregion LOADINGS Tag



        #region LOADINGS Place
        public void ShowTopLoadingPlace() => TopLoadingPlace.Start();
        public void HideTopLoadingPlace() => TopLoadingPlace.Stop();


        public void ShowBottomLoadingPlace() => BottomLoadingPlace.Start();
        public void HideBottomLoadingPlace() => BottomLoadingPlace.Stop();
        #endregion LOADINGS Tag
    }
}
