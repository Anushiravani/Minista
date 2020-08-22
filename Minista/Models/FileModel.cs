using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minista.Models
{
    public class FileModel :BaseModel
    {
        private StorageFile file_ = null;
        public StorageFile File
        {
            get { return file_; }
            set { file_ = value; }
        }

        BitmapImage thumb_;
        public BitmapImage Thumbnail
        {
            get { return thumb_; }
            set
            {
                thumb_ = value;
                OnPropertyChanged("Thumbnail");
            }
        }
        Visibility _videoVisibility = Visibility.Collapsed;
        public Visibility VideoVisibility
        {
            get { return _videoVisibility; }
            set
            {
                _videoVisibility = value;
                OnPropertyChanged("VideoVisibility");
            }
        }

        public async void SetThumb(bool skipThumbnail = false)
        {
            try
            {
                if (File == null)
                    return;
                if (File.Path.IsVideo())
                {
                    if (!skipThumbnail)
                    {
                        //var bitmap = await Helper.GetFrameAsync(File);
                        //UIThread(bitmap);
                    }
                    UIThread(null, Visibility.Visible);
                }
                else
                {
                    if (!skipThumbnail)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.SetSource(await File.GetThumbnailAsync(ThumbnailMode.SingleItem));
                        UIThread(bitmap);
                    }
                }
            }
            catch (Exception ex) { ex.PrintException("SetThumb"); }
        }

        private async void UIThread(BitmapImage bitmap = null, Visibility? visibility = null)
        {
            try
            {
                await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (bitmap != null)
                        Thumbnail = bitmap;
                    if (visibility != null)
                        VideoVisibility = visibility.Value;

                });
            }
            catch { }
        }

    }
}
