using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minista.Converters
{
    class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is string data)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var bmi = new BitmapImage(new Uri(data));

                    return new ImageBrush { ImageSource = bmi };
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
