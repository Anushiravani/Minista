using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Minista.ViewModels.Posts;
using Windows.UI.Composition;
using System.Numerics;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml.Hosting;

namespace Minista.Views.Posts
{
    public sealed partial class MultiplePostView : Page
    {
        public InstaChannel Channel;
        //public MultiplePostViewModel MultiplePostVM { get; set; } = new MultiplePostViewModel();

        readonly Compositor _compositor;
        readonly Visual _headerGridVisual;
        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static MultiplePostView Current;
        public MultiplePostView()
        {
            this.InitializeComponent();
            Current = this;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _headerGridVisual = HeaderGrid.GetVisual();
            //DataContext = MultiplePostVM;
            MultiplePostVM.SetView(this);
            Loaded += MultiplePostViewLoaded;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            KeyDown += OnKeyDownHandler;
            NavigationMode = e.NavigationMode;
            if (e.NavigationMode == NavigationMode.New)
            {
                GetType().RemovePageFromBackStack();
                MultiplePostVM?.ResetCache();
            }
            try
            {
                if (e.Parameter != null && e.Parameter is InstaChannel channel)
                    Channel = channel;
            }
            catch { }
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
                    MultiplePostVM.RunLoadMore(true);
            }
            catch { }
        }
        private void MultiplePostViewLoaded(object sender, RoutedEventArgs e)
        {
            if (NavigationMode == NavigationMode.Back && MultiplePostVM.ChannelId != null)
            {
                if (MultiplePostVM.ChannelId.ToLower().TrimAnyBullshits() == Channel.ChannelId.ToLower().TrimAnyBullshits())
                    return;
            }
            else if (NavigationMode == NavigationMode.New)
            {
                NavigationCacheMode = NavigationCacheMode.Enabled;
                CanLoadFirstPopUp = false;
            }
            if (!CanLoadFirstPopUp)
            {
                MultiplePostVM.ResetCache();
                MultiplePostVM.ChannelId = Channel.ChannelId;
                MultiplePostVM.FirstMediaId = Channel.Media.InstaIdentifier;
                MultiplePostVM.Title = Channel.Title;
                MultiplePostVM.RunLoadMore(true);
                CanLoadFirstPopUp = true;
            }
        }

        private void LVPostsLoaded(object sender, RoutedEventArgs e)
        {
            var scroll = LVPosts.FindScrollViewer();
            MultiplePostVM.SetLV(scroll);
            if (scroll != null)
                scroll.ViewChanging += ScrollViewViewChanging;
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
                    ToggleHeaderGridAnimation(false);
                }
                else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
                {
                    _isHideTitleGrid = false;
                    ToggleHeaderGridAnimation(true);
                }
                _lastVerticalOffset = scrollViewer.VerticalOffset;
            }
            catch { }
        }

        private async void ToggleHeaderGridAnimation(bool show)
        {
            if (_headerGridVisual == null)
                return;
            if (show)
                HeaderGrid.Visibility = Visibility.Visible;

            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _headerGridVisual.CenterPoint = new Vector3((float)HeaderGrid.ActualWidth / 2f, (float)HeaderGrid.ActualHeight / 2f, 0f);
            _headerGridVisual.StartAnimation("Scale.X", scaleAnimation);
            _headerGridVisual.StartAnimation("Scale.Y", scaleAnimation);
            try
            {
                if (!show)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await Task.Delay(510);
                        HeaderGrid.Visibility = Visibility.Collapsed;
                    });

                }
            }
            catch { }
        }


        #region LOADINGS
        public void ShowBottomLoading() => BottomLoading.Start();
        public void HideBottomLoading() => BottomLoading.Stop();
        #endregion LOADINGS
    }
}
