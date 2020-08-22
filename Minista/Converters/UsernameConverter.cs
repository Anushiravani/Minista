using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class UsernameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is string data)
            {
                if (!string.IsNullOrEmpty(data))
                    return data.Truncate(8);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
