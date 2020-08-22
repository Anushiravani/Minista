using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UICompositionAnimations.Enums;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Minista.Controls
{
    public sealed partial class LockControl : UserControl
    {
        readonly private DispatcherTimer Timer = new DispatcherTimer();
        private int Interval = 0;
        public LockControl()
        {
            this.InitializeComponent();
            Timer.Tick += OnTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
            Loaded += LockControl_Loaded;
        }

        private void LockControl_Loaded(object sender, RoutedEventArgs e)
        {
            try 
            {
                CTransform.TranslateX = SettingsHelper.Settings.LockControlX;
                CTransform.TranslateY = SettingsHelper.Settings.LockControlY;
            }
            catch { }
        }

        private void Manipulator_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //if (e.Position.X > Width - ResizeRectangle.Width && e.Position.Y > Height - ResizeRectangle.Height) _isResizing = true;
            //else _isResizing = false;
        }

        private void Manipulator_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //if (_isResizing)
            //{
            //    //Width += e.Delta.Translation.X;
            //    //Height += e.Delta.Translation.Y;
            //}
            //else
            {
                CTransform.TranslateX += e.Delta.Translation.X;

                CTransform.TranslateY += e.Delta.Translation.Y;

                try
                {
                    SettingsHelper.Settings.LockControlX = CTransform.TranslateX;
                    SettingsHelper.Settings.LockControlY = CTransform.TranslateY;
                }
                catch { }
                //Canvas.SetLeft(this, Canvas.GetLeft(this) + e.Delta.Translation.X);
                //Canvas.SetTop(this, Canvas.GetTop(this) + e.Delta.Translation.Y);
            }
        }
        double CalculateXPosition(double x, double parentControlWidth, double controlWidth)
        {
            var n = x /** parentControlWidth*/;

            if (n + controlWidth > parentControlWidth)
            {
                var more = parentControlWidth - (n + controlWidth);
                return n + more;
            }
            else if (n - controlWidth < 0)
            {
                return n + controlWidth;
            }
            return n;
        }
        double CalculateYPosition(double y, double parentControlHeight, double controlHeight)
        {
            var n = y /** parentControlHeight*/;
            if (n + controlHeight > parentControlHeight)
            {
                var more = parentControlHeight - (n + controlHeight);
                return n + more;
            }
            else if (n - controlHeight < 0)
            {

                return n + controlHeight;
            }
            return n;
        }
        private void OnTimerTick(object sender, object e)
        {
            try 
            {
                if (Interval > 3)
                {
                    if (Opacity == 1)
                    {
                        Timer.Stop();

                        this.Animation(FrameworkLayer.Xaml)
                            .Scale(1.2, 1.0, Easing.QuadraticEaseInOut)
                            .Duration(100)
                            .Start();

                        this.Animation(FrameworkLayer.Xaml)
                            .Opacity(1, 0.6, Easing.CircleEaseOut)
                            .Scale(1.2, 1.0, Easing.QuadraticEaseInOut)
                            .Duration(400)
                            .Start();
                    }
                }
                Interval++;
            }
            catch { }
        }

        private void LockButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Opacity != 1.0)
                {
                    Interval = 0;

                    this.Animation(FrameworkLayer.Xaml)
                        .Scale(1, 1.2, Easing.QuadraticEaseInOut)
                        .Duration(100)
                        .Start();


                    this.Animation(FrameworkLayer.Xaml)
                        .Opacity(0.6, 1.0, Easing.CircleEaseOut)
                        .Scale(1.2, 1.0, Easing.QuadraticEaseInOut)
                        .Duration(400)
                        .Start(); 
                    Timer.Start();
                }
                else
                {
                    Helper.Passcode.Lock();
                }
            }
            catch { }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LockButtonClick(null, null);
        }
    }
}
