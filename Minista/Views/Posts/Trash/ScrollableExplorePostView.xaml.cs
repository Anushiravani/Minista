using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
//using LottieUWP;
using Minista.ItemsGenerators;
using Minista.Models.Main;
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
    public sealed partial class ScrollableExplorePostView : Page
    {
        private readonly Compositor _compositor;
        private readonly Visual _goUpButtonVisual;
        private ScrollViewer Scroll;

        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private int SelectedIndex = -1;
        private ExploreClusterGenerator ExploreGenerator;
        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static ScrollableExplorePostView Current;
        public ScrollableExplorePostView()
        {
            InitializeComponent();
            Current = this;
            Loaded += ScrollableHashtagPostViewLoaded;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _goUpButtonVisual = GoUpButton.GetVisual();
            SetUpPageAnimation();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationMode = e.NavigationMode;

            if (e.NavigationMode == NavigationMode.New)
            {
                GetType().RemovePageFromBackStack();
                try
                {
                    LVPosts.ItemsSource = null;
                }
                catch { }
                //ScrollableExplorePostVM?.ExploreGenerator?.ResetCache();
            }
            try
            {
                if (e.Parameter != null && e.Parameter is object[] obj && obj?.Length == 2)
                {
                    if (obj[0] is ExploreClusterGenerator generator)
                        ExploreGenerator = generator;
                    SelectedIndex = (int)obj[1];
                }
            }
            catch { }
        }
        private void ScrollableHashtagPostViewLoaded(object sender, RoutedEventArgs e)
        {
            ToggleGoUpButtonAnimation(false);
            Loaded -= ScrollableHashtagPostViewLoaded;
            return;
            //LoadData();
        }
        public void SetData(ExploreClusterGenerator generator, int index)
        {
            ExploreGenerator = generator;
            SelectedIndex = index;

            LoadData();
        }
        private async void LoadData()
        {
            try
            {
                if (ExploreGenerator == null) return;
                if (NavigationMode == NavigationMode.Back && ScrollableExplorePostVM.ExploreGenerator != null)
                {
                    if (ScrollableExplorePostVM.ExploreGenerator.ClusterId.ToLower().TrimAnyBullshits() == ExploreGenerator.ClusterId.ToLower().TrimAnyBullshits())
                        return;
                }
                else if (NavigationMode == NavigationMode.New)
                {
                    NavigationCacheMode = NavigationCacheMode.Enabled;
                    CanLoadFirstPopUp = false;
                }
                if (!CanLoadFirstPopUp)
                {
                    if (Scroll == null)
                    {
                        Scroll = LVPosts.FindScrollViewer();
                        if (Scroll != null)
                            Scroll.ViewChanging += ScrollViewViewChanging;
                    }
                    //LVPosts.ItemsSource = null;
                    //if (ScrollableExplorePostVM.ExploreGenerator != null)
                    //    ScrollableExplorePostVM.ExploreGenerator.ResetCache();
                    await Task.Delay(100);
                    ScrollableExplorePostVM.ExploreGenerator = ExploreGenerator;
                    await Task.Delay(100);
                    LVPosts.ItemsSource = ScrollableExplorePostVM.ExploreGenerator.Items;
                    //LVPosts.ItemsSource = ScrollableExplorePostVM.ExploreGenerator.ItemsX;
                    await Task.Delay(500);
                    if (Scroll != null)
                        Scroll.ViewChanging += ScrollableExplorePostVM.ExploreGenerator.ScrollViewChanging;
                    if(SelectedIndex != -1)
                    LVPosts.ScrollIntoView(ScrollableExplorePostVM.ExploreGenerator.Items[SelectedIndex]);
                    //LVPosts.ScrollIntoView(ScrollableExplorePostVM.ExploreGenerator.ItemsX[SelectedIndex]);

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
                    LVPosts.ScrollIntoView(ScrollableExplorePostVM.ExploreGenerator.Items[SelectedIndex]);
            }
            catch { }
        }
        private void LVPostsRefreshRequested(object sender, EventArgs e)
        {
            if (ScrollableExplorePostVM.ExploreGenerator != null)
                ScrollableExplorePostVM.ExploreGenerator.RunLoadMore(true);
        }


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
                }
                else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
                {
                    _isHideTitleGrid = false;
                    ToggleGoUpButtonAnimation(true);
                }
                if (scrollViewer.VerticalOffset >= 0 && scrollViewer.VerticalOffset <= 1)
                {
                    _isHideTitleGrid = true;
                    ToggleGoUpButtonAnimation(false);
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
