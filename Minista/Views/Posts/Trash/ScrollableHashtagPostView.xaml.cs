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
    public sealed partial class ScrollableHashtagPostView : Page
    {
        private readonly Compositor _compositor;
        private readonly Visual _goUpButtonVisual;
        private ScrollViewer Scroll;

        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private int SelectedIndex = -1;
        private InstaHashtag Hashtag;
        private HashtagsRecentGenerator HashtagsRecentGenerator;
        private HashtagsTopGenerator HashtagsTopGenerator;
        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static ScrollableHashtagPostView Current;
        public ScrollableHashtagPostView()
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
                LVPosts.ItemsSource = null;
            }

            try
            {
                if (e.Parameter != null && e.Parameter is object[] obj && obj?.Length == 3)
                {
                    if (obj[0] is InstaHashtag hashtag)
                        Hashtag = hashtag;
                    if (obj[1] is HashtagsRecentGenerator generator)
                    {
                        HashtagsRecentGenerator = generator;
                        HashtagsTopGenerator = null;
                    }
                    else if (obj[1] is HashtagsTopGenerator generatorX)
                    {
                        HashtagsRecentGenerator = null;
                        HashtagsTopGenerator = generatorX;
                    }
                    SelectedIndex = (int)obj[2];
                }
            }
            catch { }
        }
        private async void ScrollableHashtagPostViewLoaded(object sender, RoutedEventArgs e)
        {
            ToggleGoUpButtonAnimation(false);
            try
            {
                if (NavigationMode == NavigationMode.Back && ScrollableHashtagPostVM.Hashtag != null)
                {
                    if (ScrollableHashtagPostVM.Hashtag.Name.ToLower().TrimAnyBullshits() == Hashtag.Name.ToLower().TrimAnyBullshits())
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
                    ScrollableHashtagPostVM.ResetCache();
                    await Task.Delay(100);
                    if (HashtagsRecentGenerator != null)
                    {
                        ScrollableHashtagPostVM.SetInfo(Hashtag, HashtagsRecentGenerator, Scroll);
                        LVPosts.ItemsSource = ScrollableHashtagPostVM.HashtagsRecentGenerator.Items;
                        //LVPosts.ItemsSource = ScrollableHashtagPostVM.HashtagsRecentGenerator.ItemsX;
                        await Task.Delay(500);
                        LVPosts.ScrollIntoView(ScrollableHashtagPostVM.HashtagsRecentGenerator.Items[SelectedIndex]);
                        //LVPosts.ScrollIntoView(ScrollableHashtagPostVM.HashtagsRecentGenerator.ItemsX[SelectedIndex]);

                    }
                    else
                    {
                        if (HashtagsTopGenerator != null)
                        {
                            ScrollableHashtagPostVM.SetInfo(Hashtag, HashtagsTopGenerator, Scroll);
                            LVPosts.ItemsSource = ScrollableHashtagPostVM.HashtagsTopGenerator.Items;
                            //LVPosts.ItemsSource = ScrollableHashtagPostVM.HashtagsTopGenerator.ItemsX;
                            await Task.Delay(500);
                            LVPosts.ScrollIntoView(ScrollableHashtagPostVM.HashtagsTopGenerator.Items[SelectedIndex]);
                            //LVPosts.ScrollIntoView(ScrollableHashtagPostVM.HashtagsTopGenerator.ItemsX[SelectedIndex]);

                        }
                    }
                    CanLoadFirstPopUp = true;
                }
            }
            catch { }
        }

        private void LVPostsRefreshRequested(object sender, EventArgs e)
        {
            if (ScrollableHashtagPostVM.HashtagsRecentGenerator != null)
                ScrollableHashtagPostVM.HashtagsRecentGenerator.RunLoadMore(true);
            else if (ScrollableHashtagPostVM.HashtagsTopGenerator != null)
                ScrollableHashtagPostVM.HashtagsTopGenerator.RunLoadMore(true);
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
