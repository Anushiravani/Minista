using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;
using System.Linq;
using Windows.UI.Xaml;

namespace Minista.Converters
{
    class UserTagCarouselItemVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            var data = value as InstaCarouselItem;
            if (data != null)
                return data.UserTags?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
