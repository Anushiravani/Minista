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

namespace Minista.Views.Settings
{
    public sealed partial class NotificationsView : Page
    {
        public static NotificationsView Current;
        public NotificationsViewModel NotificationsVM { get; set; } = new NotificationsViewModel();
        public NotificationsView()
        {
            this.InitializeComponent();
            DataContext = NotificationsVM;
            Current = this;
            Loaded += NotificationsViewLoaded;
        }

        private void NotificationsViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationsVM.RunLoadMore();
            }
            catch { }
        }

        public void ShowTopLoading() => TopLoading.Start();
        public void HideTopLoading() => TopLoading.Stop();

        private void ToggleMenuToggled(object sender, RoutedEventArgs e)
        { 

        }
    }
}
