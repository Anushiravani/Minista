using Minista.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace Minista.Views.Uploads
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UploadView : Page
    {
        public ObservableCollection<UploadUc> UploadUcList = new ObservableCollection<UploadUc>();
        public ObservableCollection<UploadUcItem> UploadUcListX = new ObservableCollection<UploadUcItem>();
        public static UploadView Current;

        private readonly VideoConverterX Convertions = new VideoConverterX();
        public UploadView()
        {
            this.InitializeComponent();
            Current = this;
            LV.ItemsSource = UploadUcListX;
            Loaded += UploadView_Loaded;
            FlipView.ItemsSource = UploadUcList;
            Convertions.OnText += ConvertionsOnText;
            Convertions.OnOutput += ConvertionsOnOutput;
        }

        private void UploadView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Helper.CreateCachedFolder();
                Helper.CreateCachedFolder();
                Helper.CreateCachedFolder();
            }
            catch { }
        }

        private void ImportButtonClick(object sender, RoutedEventArgs e) => ImportFiles();

        private async void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UploadUcList.Count == 0)
                {
                    Helper.ShowNotify("Please select at least one image or video.", 5500);
                    return;
                }
                int index = 0;
                bool editing = false;
                int ix = 0;
                foreach (var item in UploadUcList)
                {
                    if (item.Editing)
                    {
                        editing = true;
                        index = ix;
                        break;
                    }
                    ix++;
                }
                if (editing)
                {
                    Helper.ShowNotify("Some of your photo(s)/video(s) needs edit, edit them first and then try again.", 5500);
                    LV.SelectedIndex = index;
                    //FlipView.SelectedIndex = index;
                    return;
                }
                var list = new List<StorageUploadItem>();
                foreach (var item in UploadUcList)
                    list.Add(item.UploadItem);
                ConvertionGrid.Visibility = Visibility.Visible;
                LoadingUc.Start();
                var converted = await Convertions.ConvertFilesAsync(list);

                ConvertionGrid.Visibility = Visibility.Collapsed;
                LoadingUc.Stop();
                var su = new StorageUpload();
                su.SetUploadItem(converted, CaptionText.Text);
                Helper.ShowNotify("Convertion is done now you can use Minista\r\n" +
                    "We will upload your media(s) in background.", 6000);
                Helpers.NavigationService.GoBack();
            }
            catch { }
        }

        private async void ConvertionsOnOutput(string text)
        {
            try
            {
                await Helper.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ResultText.Text = text;
                });
            }
            catch { }
        }

        private async void ConvertionsOnText(string text)
        {
            try
            {
                await Helper.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ContentText.Text = text;
                });
            }
            catch { }
        }
        private async void ImportFiles(bool appendFiles = false)
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".bmp");
                //openPicker.FileTypeFilter.Add(".gif");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".mp4");
                //openPicker.FileTypeFilter.Add(".mkv");
                var files = await openPicker.PickMultipleFilesAsync();
                if (files == null) return;
                if (files.Count > 0)
                {
                    if (!appendFiles)
                    {
                        UploadUcList.Clear();
                        UploadUcListX.Clear();
                    }
                    else
                    {
                        UploadUcListX.RemoveAt(UploadUcListX.Count - 1);
                    }
                    foreach (var file in files)
                    {
                        //var item = new UploadUcItem();
                        var uc = new UploadUc
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        await uc.SetFileAsync(file);
                        //item.UploadUc = uc;
                        //UploadUcListX.Add(item);
                        //item.SetThumbIfExists();
                        UploadUcList.Add(uc);
                    }
                    if (files.Count < 9)
                    {
                        var item = new UploadUcItem
                        {
                            VideoVisibility = Visibility.Collapsed,
                            PlusVisibility = Visibility.Visible
                        };
                        UploadUcListX.Add(item);
                    }
                }
            }
            catch (Exception ex) { ex.PrintException("ImportButtonClick"); }

        }
        private void LVItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem != null)
                {
                    var item = e.ClickedItem as UploadUcItem;
                    if (item != null)
                    {
                        if (item.PlusVisibility == Visibility.Visible)
                            ImportFiles(true);
                        else
                        {

                        }
                    }
                }
            }
            catch { }
        }
    }
}
