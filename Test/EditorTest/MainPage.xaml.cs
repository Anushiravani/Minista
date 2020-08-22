using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EditorTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void EditLocal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker fo = new FileOpenPicker();
            fo.FileTypeFilter.Add(".png");
            fo.FileTypeFilter.Add(".jpg");
            fo.SuggestedStartLocation = PickerLocationId.Desktop;

            var f = await fo.PickSingleFileAsync();
            if (f != null)
               await EditorStudio.LoadFile(f);
        }
    }
}
