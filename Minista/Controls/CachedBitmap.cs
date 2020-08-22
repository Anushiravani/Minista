using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net;
using Windows.Storage;
using Minista.Helpers;
using System.ComponentModel;

namespace Minista.Controls
{
    public class CachedBitmap : Control, /*BaseModel*/INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        private async Task<StorageFolder> CachedFolder() => await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedFiles");
        private Uri imgUri_;
        public Uri Source
        {
            get
            {
                return imgUri_;
            }
            set
            {
                GetImageAsync(value);
            }
        }
        //public static readonly DependencyProperty SourceProperty =
        //    DependencyProperty.Register("Source",
        //        typeof(Uri),
        //        typeof(CachedBitmap),
        //        new PropertyMetadata(null));
        async void GetImageAsync(Uri uri)
        {
            try
            {
                var name = uri.AbsoluteUri;
                name = name.Substring(name.IndexOf("/") + 1);
                name = WebUtility.UrlDecode(name);
                name = Uri.UnescapeDataString(name);
                var folder = await CachedFolder();
                var files = await folder.GetFilesAsync();
                for (int i = 0; i < files.Count; i++)
                {
                    try
                    {
                        if (files[i].Name.ToLower() == name.ToLower())
                        {
                            Update(new Uri(files[i].Path, UriKind.RelativeOrAbsolute));
                            return;
                        }
                    }
                    catch { }
                }
                try
                {
                    var dl = await HttpHelper.DownloadFileAsync(uri, name, folder);
                    Update(dl);
                }
                catch { }
            }
            catch { Update(uri); }
        }
        void Update(Uri uri)
        {
            imgUri_ = uri;
            OnPropertyChanged("Source");
        }
    }
}
