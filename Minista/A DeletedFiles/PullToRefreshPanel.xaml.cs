using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Minista.Controls
{
    public sealed partial class PullToRefreshPanel : UserControl/*, INotifyPropertyChanged*/
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChangedX(string memberName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        //}
        /// <summary>
        /// Identifies the <see cref="OverscrollLimit"/> property.
        /// </summary>
        public static readonly DependencyProperty OverscrollLimitProperty =
            DependencyProperty.Register(nameof(OverscrollLimit), typeof(double), typeof(PullToRefreshPanel), new PropertyMetadata(0.4, OverscrollLimitPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="PullThreshold"/> property.
        /// </summary>
        public static readonly DependencyProperty PullThresholdProperty =
            DependencyProperty.Register(nameof(PullThreshold), typeof(double), typeof(PullToRefreshPanel), new PropertyMetadata(100.0));

        /// <summary>
        /// Identifies the <see cref="RefreshCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(PullToRefreshPanel), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RefreshIntentCanceledCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty RefreshIntentCanceledCommandProperty =
            DependencyProperty.Register(nameof(RefreshIntentCanceledCommand), typeof(ICommand), typeof(PullToRefreshPanel), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="MainContent"/> property.
        /// </summary>
        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register(nameof(MainContent), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RefreshIndicatorContent"/> property.
        /// </summary>
        public static readonly DependencyProperty RefreshIndicatorContentProperty =
            DependencyProperty.Register(nameof(RefreshIndicatorContent), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PullToRefreshLabel"/> property.
        /// </summary>
        public static readonly DependencyProperty PullToRefreshLabelProperty =
            DependencyProperty.Register(nameof(PullToRefreshLabel), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata("Pull To Refresh", OnPullToRefreshLabelChanged));

        /// <summary>
        /// Identifies the <see cref="ReleaseToRefreshLabel"/> property.
        /// </summary>
        public static readonly DependencyProperty ReleaseToRefreshLabelProperty =
            DependencyProperty.Register(nameof(ReleaseToRefreshLabel), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata("Release to Refresh", OnReleaseToRefreshLabelChanged));

        /// <summary>
        /// Identifies the <see cref="PullToRefreshContent"/> property.
        /// </summary>
        public static readonly DependencyProperty PullToRefreshContentProperty =
            DependencyProperty.Register(nameof(PullToRefreshContent), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata("Pull To Refresh"));

        /// <summary>
        /// Identifies the <see cref="ReleaseToRefreshContent"/> property.
        /// </summary>
        public static readonly DependencyProperty ReleaseToRefreshContentProperty =
            DependencyProperty.Register(nameof(ReleaseToRefreshContent), typeof(object), typeof(PullToRefreshPanel), new PropertyMetadata("Release to Refresh"));

        /// <summary>
        /// IsPullToRefreshWithMouseEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsPullToRefreshWithMouseEnabledProperty =
            DependencyProperty.Register(nameof(IsPullToRefreshWithMouseEnabled), typeof(bool), typeof(PullToRefreshPanel), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="UseRefreshContainerWhenPossible"/> dependency property
        /// </summary>
        public static readonly DependencyProperty UseRefreshContainerWhenPossibleProperty =
            DependencyProperty.Register(nameof(UseRefreshContainerWhenPossible), typeof(bool), typeof(PullToRefreshPanel), new PropertyMetadata(false, OnUseRefreshContainerWhenPossibleChanged));

        /// <summary>
        /// Gets a value indicating whether <see cref="RefreshContainer"/> is supported
        /// </summary>
        public static bool IsRefreshContainerSupported { get; } = ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.RefreshContainer");


        private ScrollViewer ScrollViewer;
        private CompositeTransform _contentTransform;
        private ScrollBar _scrollerVerticalScrollBar;
        private double _lastOffset = 0.0;
        private double _pullDistance = 0.0;
        private DateTime _lastRefreshActivation = default(DateTime);
        private bool _refreshActivated = false;
        private bool _refreshIntentCanceled = false;
        private double _overscrollMultiplier;
        private bool _isManipulatingWithMouse;
        private double _startingVerticalOffset;

        private bool UsingRefreshContainer => IsRefreshContainerSupported && UseRefreshContainerWhenPossible;

        /// <summary>
        /// Gets or sets a value indicating whether the HamburgerMenu should use the NavigationView when possible (Fall Creators Update and above)
        /// When set to true and the device supports NavigationView, the HamburgerMenu will use a template based on NavigationView
        /// </summary>
        public bool UseRefreshContainerWhenPossible
        {
            get { return (bool)GetValue(UseRefreshContainerWhenPossibleProperty); }
            set { SetValue(UseRefreshContainerWhenPossibleProperty, value); }
        }

        /// <summary>
        /// Occurs when the user has requested content to be refreshed
        /// </summary>
        public event EventHandler RefreshRequested;

        /// <summary>
        /// Occurs when the user has cancels an intent for the content to be refreshed
        /// </summary>
        public event EventHandler RefreshIntentCanceled;

        /// <summary>
        /// Occurs when listview overscroll distance is changed
        /// </summary>
        public event EventHandler<RefreshProgressEventArgs> PullProgressChanged;

        public PullToRefreshPanel()
        {
            this.InitializeComponent();
            //DataContext = this;
            SizeChanged += RefreshableListView_SizeChanged;

        }

        private void MContentLoaded(object sender, RoutedEventArgs e)
        {
            if(MainContent is AdaptiveGridView agv)
            {
                try
                {
                    try
                    {
                        agv.ScrollChanged -= AdaptiveGridViewScrollChanged; 
                    }
                    catch { }

                    agv.ScrollChanged += AdaptiveGridViewScrollChanged;
                }
                catch { }
            }
            //else if (MainContent is ListView lv)
            //{
            //    try
            //    {
            //        try
            //        {
            //            lv.ScrollChanged -= AdaptiveGridViewScrollChanged;
            //        }
            //        catch { }

            //        lv.ScrollChanged += AdaptiveGridViewScrollChanged;
            //    }
            //    catch { }
            //}
        }

        private void AdaptiveGridViewScrollChanged(object sender, ScrollViewer e)
        {
            if (e == null)
                return;
            if (ScrollViewer != null) return;

            ScrollViewer = e;
            OnApplyTemplateX();
        }

        /// <summary>
        /// Handler for SizeChanged event, handles cliping
        /// </summary>
        private void RefreshableListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding
        /// layout pass) call <see cref="OnApplyTemplate"/>. In simplest terms, this means the method
        /// is called just before a UI element displays in an application. Override this
        /// method to influence the default post-template logic of a class.
        /// </summary>
        void OnApplyTemplateX()
        {
            if (ScrollViewer != null)
            {
                ScrollViewer.Loaded -= Scroller_Loaded;
                ScrollViewer.DirectManipulationCompleted -= Scroller_DirectManipulationCompleted;
                ScrollViewer.DirectManipulationStarted -= Scroller_DirectManipulationStarted;
            }

            if (RefreshIndicator != null)
            {
                RefreshIndicator.SizeChanged -= RefreshIndicatorBorder_SizeChanged;
            }

            if (Root != null)
            {
                Root.ManipulationStarted -= Scroller_ManipulationStarted;
                Root.ManipulationCompleted -= Scroller_ManipulationCompleted;
            }


            //if (MainContent is ListViewBase lvb)
            //    ScrollViewer = lvb.FindScrollViewer();
            //if (MainContent is GridView gv)
            //    ScrollViewer = gv.FindScrollViewer();
            //if (MainContent is ListView lv)
            //    ScrollViewer = lv.FindScrollViewer();
            //if (MainContent is AdaptiveGridView agv)


            if(ScrollViewer==null)
                ScrollViewer = (MainContent as UIElement).FindScrollViewer();

            if (Root != null &&
                ScrollViewer != null &&
                RefreshIndicator != null &&
                RefreshIndicatorTransform != null && PullAndReleaseIndicatorContent != null)
            {
                ScrollViewer.Loaded += Scroller_Loaded;

                SetupMouseMode();

                ScrollViewer.DirectManipulationCompleted += Scroller_DirectManipulationCompleted;
                ScrollViewer.DirectManipulationStarted += Scroller_DirectManipulationStarted;

                if (PullAndReleaseIndicatorContent != null)
                {
                    PullAndReleaseIndicatorContent.Visibility = RefreshIndicatorContent == null ? Visibility.Visible : Visibility.Collapsed;
                }

                RefreshIndicator.SizeChanged += RefreshIndicatorBorder_SizeChanged;

                _overscrollMultiplier = OverscrollLimit * 8;
            }

            base.OnApplyTemplate();
        }



        private void Scroller_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // Other input are already managed by the scroll viewer
            if (/*e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse &&*/ IsPullToRefreshWithMouseEnabled)
            {
                if (ScrollViewer.VerticalOffset < 1)
                {
                    DisplayPullToRefreshContent();
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                    _isManipulatingWithMouse = true;
                }

                _startingVerticalOffset = ScrollViewer.VerticalOffset;
                Root.ManipulationDelta -= Scroller_ManipulationDelta;
                Root.ManipulationDelta += Scroller_ManipulationDelta;
            }
        }

        private void Scroller_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Root.ManipulationDelta -= Scroller_ManipulationDelta;

            if (!IsPullToRefreshWithMouseEnabled)
            {
                return;
            }

            OnManipulationCompleted();
        }

        private void Scroller_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!IsPullToRefreshWithMouseEnabled || _contentTransform == null)
            {
                return;
            }

            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                return;
            }

            if (e.Cumulative.Translation.Y <= 0 || ScrollViewer.VerticalOffset >= 1)
            {
                ScrollViewer.ChangeView(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset - e.Delta.Translation.Y, 1);
                return;
            }

            if (_startingVerticalOffset >= 1)
            {
                return;
            }

            // content is not "moved" automagically by the scrollviewer in this case
            // so we need to apply our own transformation.
            // and to do so we use a little Sin Easing.

            // how much "drag" to go to the max translation
            var mouseMaxDragDistance = 100;

            // make it harder to drag (life is not easy)
            double translationToUse = e.Cumulative.Translation.Y / 3;
            var deltaCumulative = Math.Min(translationToUse, mouseMaxDragDistance) / mouseMaxDragDistance;

            // let's do some quartic ease-out
            double f = deltaCumulative - 1;
            var easing = 1 + (f * f * f * (1 - deltaCumulative));

            var maxTranslation = 150;
            _contentTransform.TranslateY = easing * maxTranslation;
        }

        private void RefreshIndicatorBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
        }

        private void Scroller_DirectManipulationStarted(object sender, object e)
        {
            // sometimes the value gets stuck at 0.something, so checking if less than 1
            if (ScrollViewer.VerticalOffset < 1)
            {
                OnManipulationCompleted();
                DisplayPullToRefreshContent();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        /// <summary>
        /// Display the pull to refresh content
        /// </summary>
        private void DisplayPullToRefreshContent()
        {
            if (RefreshIndicatorContent == null)
            {

                if (PullAndReleaseIndicatorContent != null)
                {
                    PullAndReleaseIndicatorContent.Content = PullToRefreshContent;
                }
            }
        }

        private void Scroller_DirectManipulationCompleted(object sender, object e)
        {
            OnManipulationCompleted();
            Root.ManipulationMode = ManipulationModes.System;
        }

        /// <summary>
        /// Method called at the end of manipulation to clean up everything
        /// </summary>
        private void OnManipulationCompleted()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
            if (_contentTransform != null)
            {
                _contentTransform.TranslateY = 0;

            }

            if (_refreshActivated)
            {
                RefreshRequested?.Invoke(this, EventArgs.Empty);
                if (RefreshCommand != null && RefreshCommand.CanExecute(null))
                {
                    RefreshCommand.Execute(null);
                }
            }
            else if (_refreshIntentCanceled)
            {
                RefreshIntentCanceled?.Invoke(this, EventArgs.Empty);
                if (RefreshIntentCanceledCommand != null && RefreshIntentCanceledCommand.CanExecute(null))
                {
                    RefreshIntentCanceledCommand.Execute(null);
                }
            }

            _lastOffset = 0;
            _pullDistance = 0;
            _refreshActivated = false;
            _refreshIntentCanceled = false;
            _lastRefreshActivation = default(DateTime);
            _isManipulatingWithMouse = false;

            PullProgressChanged?.Invoke(this, new RefreshProgressEventArgs { PullProgress = 0 });
            PullAndReleaseIndicatorContent.Content = null;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            // if started navigating down, cancel the refresh
            if (ScrollViewer.VerticalOffset > 1)
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
                if (_contentTransform != null)
                {
                    _contentTransform.TranslateY = 0;

                }

                _refreshActivated = false;
                _lastRefreshActivation = default(DateTime);

                PullProgressChanged?.Invoke(this, new RefreshProgressEventArgs { PullProgress = 0 });
                _isManipulatingWithMouse = false;

                return;
            }

            if (_contentTransform == null)
            {
                var itemsPanel = VisualTreeHelper.GetChild(MainContent as UIElement, 0) as UIElement;
                //var headerContent = VisualTreeHelper.GetChild(MainContent, 0) as UIElement;
                //var itemsPanel = VisualTreeHelper.GetChild(MainContent, 1) as UIElement;
                //var footerContent = VisualTreeHelper.GetChild(MainContent, 2) as UIElement;

                //if (_headerTransform == null && VisualTreeHelper.GetChildrenCount(headerContent) > 0)
                //{
                //    if (headerContent != null)
                //    {
                //        _headerTransform = new CompositeTransform();
                //        headerContent.RenderTransform = _headerTransform;
                //    }
                //}

                //if (_footerTransform == null && VisualTreeHelper.GetChildrenCount(footerContent) > 0)
                //{
                //    if (footerContent != null)
                //    {
                //        _footerTransform = new CompositeTransform();
                //        footerContent.RenderTransform = _footerTransform;
                //    }
                //}

                if (itemsPanel == null)
                {
                    return;
                }

                _contentTransform = new CompositeTransform();
                itemsPanel.RenderTransform = _contentTransform;
            }

            Rect elementBounds = (MainContent as UIElement).TransformToVisual(Root).TransformBounds(default(Rect));

            // content is not "moved" automagically by the scrollviewer in this case
            // so we apply our own transformation too and need to take it in account.
            if (_isManipulatingWithMouse)
            {
                elementBounds = _contentTransform.TransformBounds(elementBounds);
            }

            var offset = elementBounds.Y;
            var delta = offset - _lastOffset;
            _lastOffset = offset;

            _pullDistance += delta * _overscrollMultiplier;

            if (_isManipulatingWithMouse)
            {
                _pullDistance = 2 * offset;
            }

            if (_pullDistance > 0)
            {
                if (!_isManipulatingWithMouse)
                {
                    _contentTransform.TranslateY = _pullDistance - offset;

                }

                if (_isManipulatingWithMouse)
                {
                    RefreshIndicatorTransform.TranslateY = _pullDistance - offset
                                                        - RefreshIndicator.ActualHeight;
                }
                else
                {
                    RefreshIndicatorTransform.TranslateY = _pullDistance
                                                        - RefreshIndicator.ActualHeight;
                }
            }
            else
            {
                if (!_isManipulatingWithMouse)
                {
                    _contentTransform.TranslateY = 0;

                   
                }

                RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
            }

            double pullProgress;
            if (_pullDistance >= PullThreshold)
            {
                _lastRefreshActivation = DateTime.Now;
                _refreshActivated = true;
                _refreshIntentCanceled = false;
                pullProgress = 1.0;
                if (RefreshIndicatorContent == null)
                {
                    if (PullAndReleaseIndicatorContent != null)
                    {
                        PullAndReleaseIndicatorContent.Content = ReleaseToRefreshContent;
                    }
                }
            }
            else if (_lastRefreshActivation != DateTime.MinValue)
            {
                TimeSpan timeSinceActivated = DateTime.Now - _lastRefreshActivation;

                // if more then a second since activation, deactivate
                if (timeSinceActivated.TotalMilliseconds > 1000)
                {
                    _refreshIntentCanceled |= _refreshActivated;
                    _refreshActivated = false;
                    _lastRefreshActivation = default(DateTime);
                    pullProgress = _pullDistance / PullThreshold;
                    if (RefreshIndicatorContent == null)
                    {

                        if (PullAndReleaseIndicatorContent != null)
                        {
                            PullAndReleaseIndicatorContent.Content = PullToRefreshContent;
                        }
                    }
                }
                else
                {
                    pullProgress = 1.0;
                    _refreshIntentCanceled |= _refreshActivated;
                }
            }
            else
            {
                pullProgress = _pullDistance / PullThreshold;
                _refreshIntentCanceled |= _refreshActivated;
            }

            PullProgressChanged?.Invoke(this, new RefreshProgressEventArgs { PullProgress = pullProgress });
        }

        private void Scroller_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollerVerticalScrollBar = ScrollViewer.FindDescendantByName("VerticalScrollBar") as ScrollBar;
            _scrollerVerticalScrollBar.PointerEntered += ScrollerVerticalScrollBar_PointerEntered;
            _scrollerVerticalScrollBar.PointerExited += ScrollerVerticalScrollBar_PointerExited;
        }

        private void Scroller_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Root.ManipulationMode = ManipulationModes.System;
        }

        private void Scroller_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (IsPullToRefreshWithMouseEnabled && e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Root.ManipulationMode = ManipulationModes.TranslateY;
            }
        }

        private void ScrollerVerticalScrollBar_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Root.ManipulationMode = ManipulationModes.System;
            Root.ManipulationStarted -= Scroller_ManipulationStarted;
            Root.ManipulationCompleted -= Scroller_ManipulationCompleted;
        }

        private void ScrollerVerticalScrollBar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (IsPullToRefreshWithMouseEnabled)
            {
                Root.ManipulationStarted -= Scroller_ManipulationStarted;
                Root.ManipulationCompleted -= Scroller_ManipulationCompleted;
                Root.ManipulationStarted += Scroller_ManipulationStarted;
                Root.ManipulationCompleted += Scroller_ManipulationCompleted;
            }
        }

        private void RefreshContainer_RefreshRequested(object sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            using (var deferral = args.GetDeferral())
            {
                RefreshRequested?.Invoke(this, EventArgs.Empty);
                if (RefreshCommand != null && RefreshCommand.CanExecute(null))
                {
                    RefreshCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Overscroll Limit. Value between 0 and 1 where 1 is the height of the control. Default is 0.3
        /// </summary>
        public double OverscrollLimit
        {
            get { return (double)GetValue(OverscrollLimitProperty); }
            set { SetValue(OverscrollLimitProperty, value); }
        }

        private static void OverscrollLimitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double value = (double)e.NewValue;
            PullToRefreshPanel view = d as PullToRefreshPanel;

            if (value >= 0 && value <= 1)
            {
                view._overscrollMultiplier = value * 8;
            }
            else
            {
                throw new IndexOutOfRangeException("OverscrollCoefficient has to be a double value between 0 and 1 inclusive.");
            }
        }

        private static void OnPullToRefreshLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(PullToRefreshContentProperty, e.NewValue);
        }

        private static void OnReleaseToRefreshLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ReleaseToRefreshLabelProperty, e.NewValue);
        }

        private static void OnUseRefreshContainerWhenPossibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as PullToRefreshPanel;
            if (list == null)
            {
                return;
            }

 
        }

        /// <summary>
        /// Gets or sets the PullThreshold in pixels for when Refresh should be Requested. Default is 100
        /// </summary>
        public double PullThreshold
        {
            get { return (double)GetValue(PullThresholdProperty); }
            set { SetValue(PullThresholdProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Command that will be invoked when Refresh is requested
        /// </summary>
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Command that will be invoked when a refresh intent is cancled
        /// </summary>
        public ICommand RefreshIntentCanceledCommand
        {
            get { return (ICommand)GetValue(RefreshIntentCanceledCommandProperty); }
            set { SetValue(RefreshIntentCanceledCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Content of the Refresh Indicator
        /// </summary>
        public object RefreshIndicatorContent
        {
            get
            {
                return (object)GetValue(RefreshIndicatorContentProperty);
            }

            set
            {
                //if (_defaultIndicatorContent != null && _pullAndReleaseIndicatorContent != null)
                //{
                //    _defaultIndicatorContent.Visibility = Visibility.Collapsed;
                //}
                //else if (_defaultIndicatorContent != null)
                //{
                //    _defaultIndicatorContent.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                //}
        
                if (PullAndReleaseIndicatorContent != null)
                {
                    PullAndReleaseIndicatorContent.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                }
                if (value != null)
                {
                    PullAndReleaseIndicatorContent.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                }
                SetValue(RefreshIndicatorContentProperty, value); 
            }
        }
        public object MainContent
        {
            get
            {
                return (object)GetValue(MainContentProperty);
            }
            set
            {
               
                SetValue(MainContentProperty, value); 
            }
        }
        /// <summary>
        /// Gets or sets the label that will be shown when the user pulls down to refresh.
        /// Note: This label will only show up if <see cref="RefreshIndicatorContent" /> is null
        /// </summary>
        public string PullToRefreshLabel
        {
            get { return (string)GetValue(PullToRefreshLabelProperty); }
            set { SetValue(PullToRefreshLabelProperty, value);}
        }

        /// <summary>
        /// Gets or sets the label that will be shown when the user needs to release to refresh.
        /// Note: This label will only show up if <see cref="RefreshIndicatorContent" /> is null
        /// </summary>
        public string ReleaseToRefreshLabel
        {
            get { return (string)GetValue(ReleaseToRefreshLabelProperty); }
            set { SetValue(ReleaseToRefreshLabelProperty, value);}
        }

        /// <summary>
        /// Gets or sets the content that will be shown when the user pulls down to refresh.
        /// </summary>
        /// <remarks>
        /// This content will only show up if <see cref="RefreshIndicatorContent" /> is null
        /// </remarks>
        public object PullToRefreshContent
        {
            get { return (object)GetValue(PullToRefreshContentProperty); }
            set { SetValue(PullToRefreshContentProperty, value);  }
        }

        /// <summary>
        /// Gets or sets the content that will be shown when the user needs to release to refresh.
        /// </summary>
        /// <remarks>
        /// This content will only show up if <see cref="RefreshIndicatorContent" /> is null
        /// </remarks>
        public object ReleaseToRefreshContent
        {
            get { return (object)GetValue(ReleaseToRefreshContentProperty); }
            set { SetValue(ReleaseToRefreshContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PullToRefresh is enabled with a mouse
        /// </summary>
        public bool IsPullToRefreshWithMouseEnabled
        {
            get
            {
                return (bool)GetValue(IsPullToRefreshWithMouseEnabledProperty);
            }

            set
            {
                SetValue(IsPullToRefreshWithMouseEnabledProperty, value);
                SetupMouseMode();
            }
        }

        private void SetupMouseMode()
        {
            if (Root != null && ScrollViewer != null)
            {
                if (IsPullToRefreshWithMouseEnabled)
                {
                    Root.ManipulationStarted -= Scroller_ManipulationStarted;
                    Root.ManipulationCompleted -= Scroller_ManipulationCompleted;
                    ScrollViewer.PointerMoved -= Scroller_PointerMoved;
                    ScrollViewer.PointerExited -= Scroller_PointerExited;

                    Root.ManipulationStarted += Scroller_ManipulationStarted;
                    Root.ManipulationCompleted += Scroller_ManipulationCompleted;
                    ScrollViewer.PointerMoved += Scroller_PointerMoved;
                    ScrollViewer.PointerExited += Scroller_PointerExited;
                }
                else
                {
                    Root.ManipulationMode = ManipulationModes.System;
                    Root.ManipulationStarted -= Scroller_ManipulationStarted;
                    Root.ManipulationCompleted -= Scroller_ManipulationCompleted;
                    ScrollViewer.PointerMoved -= Scroller_PointerMoved;
                    ScrollViewer.PointerExited -= Scroller_PointerExited;
                }
            }
        }

    }
}
