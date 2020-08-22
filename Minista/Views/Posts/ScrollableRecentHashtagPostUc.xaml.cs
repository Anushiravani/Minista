using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
//using LottieUWP;
using Minista.ItemsGenerators;
using Minista.Models.Main;
using Minista.ViewModels.Infos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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


namespace Minista.Views.Posts
{
    public sealed partial class ScrollableRecentHashtagPostUc : UserControl
    {
        private readonly Compositor _compositor;
        private readonly Visual _goUpButtonVisual;
        private readonly Visual _refreshButtonVisual;
        private ScrollViewer Scroll;
        private HashtagsRecentGenerator HashtagsRecentGenerator;
        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private int SelectedIndex = -1;
        private bool CanLoadFirstPopUp = false;
        public ScrollableRecentHashtagPostUc()
        {
            this.InitializeComponent();
            Loaded += UcLoaded;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _goUpButtonVisual = GoUpButton.GetVisual();
            _refreshButtonVisual = RefreshButton.GetVisual();
            SetUpPageAnimation();
        }
        private void UcLoaded(object sender, RoutedEventArgs e)
        {
            ToggleGoUpButtonAnimation(false);
            Loaded -= UcLoaded;
            try
            {
                KeyDown -= OnKeyDownHandler;
            }
            catch { }
            KeyDown += OnKeyDownHandler;

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
        public void SetData(HashtagsRecentGenerator vm, int index)
        {
            SelectedIndex = index;
            HashtagsRecentGenerator = vm;
            try
            {
                RefreshControl.RefreshRequested -= RefreshControlRefreshRequested;
                RefreshControl.Visualizer.RefreshStateChanged -= RefreshControlRefreshStateChanged;
            }
            catch { }
            RefreshControl.RefreshRequested += RefreshControlRefreshRequested;
            if (RefreshControl.Visualizer != null)
                RefreshControl.Visualizer.RefreshStateChanged += RefreshControlRefreshStateChanged;
            LoadData();
        }
        private async void LoadData()
        {
            try
            {
                if (HashtagsRecentGenerator == null) return;
                if (!CanLoadFirstPopUp)
                {
                    try
                    {
                        LVPosts.ItemsSource = null;
                    }
                    catch { }
                    if (Scroll == null)
                    {
                        Scroll = LVPosts.FindScrollViewer();
                        if (Scroll != null)
                            Scroll.ViewChanging += ScrollViewViewChanging;
                    }
                    await Task.Delay(100);
                    LVPosts.ItemsSource = HashtagsRecentGenerator.Items;
                    await Task.Delay(500);
                    if (Scroll != null)
                        Scroll.ViewChanging += HashtagsRecentGenerator.ScrollViewChanging;
                    if (SelectedIndex != -1)
                        LVPosts.ScrollIntoView(HashtagsRecentGenerator.Items[SelectedIndex]);

                    CanLoadFirstPopUp = true;
                }
            }
            catch { }
        }
        public void ScrollTo(int index)
        {
            try
            {
                SelectedIndex = index;
                if (SelectedIndex != -1)
                    LVPosts.ScrollIntoView(HashtagsRecentGenerator.Items[SelectedIndex]);
            }
            catch { }
        }
        private void RefreshControlRefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            using (var RefreshCompletionDeferral = args.GetDeferral())
            {
                if (HashtagsRecentGenerator != null)
                    HashtagsRecentGenerator.RunLoadMore(true);
            }
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
        //private void LVPostsRefreshRequested(object sender, EventArgs e)
        //{
        //    if (ScrollableExplorePostVM.ExploreGenerator != null)
        //        ScrollableExplorePostVM.ExploreGenerator.RunLoadMore(true);
        //}


        private void ScrollViewViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                var scrollViewer = sender as ScrollViewer;

                //$"VerticalOffset: {scrollViewer.VerticalOffset}".PrintDebug();
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
                if (scrollViewer.VerticalOffset >= 0 && scrollViewer.VerticalOffset <= 1)
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
            try
            {
                var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
                scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
                scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

                _goUpButtonVisual.CenterPoint = new Vector3((float)GoUpButton.ActualWidth / 2f, (float)GoUpButton.ActualHeight / 2f, 0f);
                _goUpButtonVisual.StartAnimation("Scale.X", scaleAnimation);
                _goUpButtonVisual.StartAnimation("Scale.Y", scaleAnimation);
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

        private void GoUpButtonClick(object sender, RoutedEventArgs e)
        {
            Scroll?.ScrollToElement(0);
        }

        void SetUpPageAnimation()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new ContinuumNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            Transitions = collection;
        }

        #region LOADINGS
        public void ShowTopLoading() => TopLoading.Start();
        public void HideTopLoading() => TopLoading.Stop();


        public void ShowBottomLoading() => BottomLoading.Start();
        public void HideBottomLoading() => BottomLoading.Stop();
        #endregion LOADINGS
    }
}
