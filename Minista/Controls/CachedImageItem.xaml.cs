using InstagramApiSharp.Classes.Models;
using Minista.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace Minista.Controls
{
    public sealed partial class CachedImageItem : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public CachedImageItem()
        {
            InitializeComponent();
            DataContextChanged += CachedImage_DataContextChanged;
        }

        private void CachedImage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //args.NewValue?.GetType().PrintDebug();
            if (args.NewValue is InstaMedia media)
            {
                try
                {
                    if (media.MediaType == InstaMediaType.Carousel)

                        GetImageAsync(media.Carousel[0].Images[0].Uri);
                    else

                        GetImageAsync(media.Images[0].Uri);
                }
                catch { }
            }
            else if (args.NewValue is InstaCarouselItem carouselItem)
            {
                try
                {
                    GetImageAsync(carouselItem.Images[0].Uri);
                }
                catch { }
            }
        }

        public string Source
        {
            get
            {
                return (string)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
                //GetImageAsync(value);
            }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source",
                typeof(string),
                typeof(CachedImage),
                new PropertyMetadata(null));

        //public BitmapImage BitmapImageSource
        //{
        //    get
        //    {
        //        return (BitmapImage)GetValue(BitmapImageSourceProperty);
        //    }
        //    set
        //    {
        //        SetValue(BitmapImageSourceProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty BitmapImageSourceProperty =
        //    DependencyProperty.Register("BitmapImageSource",
        //        typeof(BitmapImage),
        //        typeof(CachedImage),
        //        new PropertyMetadata(null));
        private async Task<StorageFolder> CachedFolder() => await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedFiles");

        /*async*/ void GetImageAsync(string url)
        {
            Uri uri = new Uri(url);
            Update(uri);
            return;
            //try
            //{
            //    Helper.CreateCachedFilesFolder();
            //    var name = uri.AbsolutePath;
            //    name = name.Substring(name.LastIndexOf("/") + 1);
            //    name = WebUtility.UrlDecode(name);
            //    name = Uri.UnescapeDataString(name);
            //    var folder = await CachedFolder();
            //    var files = await folder.GetFilesAsync();
            //    for (int i = 0; i < files.Count; i++)
            //    {
            //        try
            //        {
            //            if (files[i].Name.ToLower() == name.ToLower())
            //            {
            //                Update(new Uri(files[i].Path, UriKind.RelativeOrAbsolute));
            //                return;
            //            }
            //        }
            //        catch { }
            //    }
            //    try
            //    {
            //        var dl = await HttpHelper.DownloadFileAsync(uri, name, folder);
            //        Update(dl);
            //    }
            //    catch { }
            //}
            //catch { Update(uri); }
        }
        void Update(Uri uri)
        {
            try
            {
                var BitmapImageSource = new BitmapImage(uri);

                IMGEX.Source = BitmapImageSource;
            }
            catch { }
        }
    }
}
