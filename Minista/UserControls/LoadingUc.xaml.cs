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

namespace Minista.UserControls
{
    public sealed partial class LoadingUc : UserControl
    {
        public LoadingUc() => InitializeComponent();

        public bool Started
        {
            get
            {
                return (bool)GetValue(StartedProperty);
            }
            set
            {
                if (value) Start();
                else Stop();
                SetValue(StartedProperty, value);
            }
        }
        public static readonly DependencyProperty StartedProperty =
            DependencyProperty.Register("Started",
                typeof(bool),
                typeof(LoadingUc),
                new PropertyMetadata(false));

        public void Start()
        {
            try
            {
                Visibility = Visibility.Visible;
                Busy1.IsActive = Busy2.IsActive = true;
                Show.Begin();
            }
            catch { }
        }

        public void Stop()
        {
            try
            {
                Busy1.IsActive = Busy2.IsActive = false;
                Hide.Begin();
            }
            catch { }
        }
        private void HideCompleted(object sender, object e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
