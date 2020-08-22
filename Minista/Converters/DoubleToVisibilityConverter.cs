using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
namespace Minista.Converters
{
    class DoubleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is double data)
            {
                if (data == 0)
                    return Visibility.Collapsed;
                else if (data > 0 && data <= 100)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            if (value is int data2)
            {
                if (data2 > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
