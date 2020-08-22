﻿using InstagramApiSharp.Classes.Models;
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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Minista.ViewModels.Main;
using Minista.ItemsGenerators;

namespace Minista.Views.Infos
{
    public sealed partial class LikedPostView : Page
    {
        private readonly Visual _refreshButtonVisual;

        readonly Compositor _compositor;
        readonly ImplicitAnimationCollection _elementImplicitAnimation;
        //Visual _headerGridVisual;
        private bool CanLoadFirstPopUp = false;
        NavigationMode NavigationMode;
        public static LikedPostView Current;
        public LikedPostView()
        {
            this.InitializeComponent();
            Current = this;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _elementImplicitAnimation = _compositor.CreateImplicitAnimationCollection();

            //_elementImplicitAnimation["Offset"] = CreateOffsetAnimation();
            _elementImplicitAnimation["Opacity"] = CreateOpacityAnimation();
            _refreshButtonVisual = RefreshButton.GetVisual();

            //_headerGridVisual = HeaderGrid.GetVisual();
            Loaded += LikedPostViewLoaded;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            KeyDown += OnKeyDownHandler;
            NavigationMode = e.NavigationMode;
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
        private void LikedPostViewLoaded(object sender, RoutedEventArgs e)
        {
            try
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


                if (NavigationMode == NavigationMode.New)
                {
                    GetType().RemovePageFromBackStack();
                    NavigationCacheMode = NavigationCacheMode.Enabled;
                    CanLoadFirstPopUp = false;
                }
                if (!CanLoadFirstPopUp)
                {
                    LikedPostVM.ResetCache();
                    LikedPostVM.RunLoadMore(true);
                    ScrollableLikedPostUc.SetData(LikedPostVM, -1);
                    CanLoadFirstPopUp = true;
                }
            }
            catch { }
        }
        private void RefreshControlRefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            using (var RefreshCompletionDeferral = args.GetDeferral())
                LikedPostVM.RunLoadMore(true);
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
        private void LVItemsLVLoaded(object sender, RoutedEventArgs e)
        {
            var scroll = LVItemsLV.FindScrollViewer();
            if (scroll != null)
                scroll.ViewChanging += ScrollViewViewChanging;
            LikedPostVM.SetLV(scroll);
        }

        //private void LVItemsLVRefreshRequested(object sender, EventArgs e)
        //{
        //    ExploreClusterVM.RunLoadMore(true);
        //}

        //private void LVRefreshRequested(object sender, EventArgs e)
        //{
        //    ExploreClusterVM.RunLoadMore(true);
        //}













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













        private async void LVItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is InstaMedia media)
                {
                    var index = LikedPostVM.Items.IndexOf(media);

                    //Helpers.NavigationService.Navigate(typeof(Posts.ScrollableExplorePostView),
                    //    new object[] { ExploreClusterVM, index });
                    //Helpers.NavigationService.Navigate(typeof(Posts.SinglePostView), media);

                    ScrollableLikedPostUc.Visibility = Visibility.Visible;
                    await Task.Delay(350);
                    ScrollableLikedPostUc.ScrollTo(index);
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
            offsetAnimation.Duration = TimeSpan.FromSeconds(.25);

            //Define Animation Target for this animation to animate using definition. 
            offsetAnimation.Target = "Offset";


            ScalarKeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, 0.9f);
            fadeAnimation.Duration = TimeSpan.FromSeconds(.25);
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




        private void HeaderGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                //HeaderGridLV.Height = HeaderGrid.ActualHeight;
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
