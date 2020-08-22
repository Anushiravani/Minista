using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;
using System.Linq;

namespace Minista.Converters
{
    class CarouselImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaCarousel data)
            {
                if (data?.Count > 0)
                    try
                    {
                        return new Uri(data.FirstOrDefault().Images.FirstOrDefault().Uri);
                    }
                    catch { }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
