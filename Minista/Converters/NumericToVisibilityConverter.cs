using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class NumericToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is int ix)
            {
                if (ix != default(int) && ix > 0)
                    return Visibility.Visible;
            }
            if (value is long lng)
            {
                if (lng != default(long) && lng > 0)
                    return Visibility.Visible;
            }
            else if( value is List<object> obj && obj?.Count > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
