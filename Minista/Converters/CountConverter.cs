using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    internal class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is long data)
                return $"{data.Divide()}";
            else if (value is int data2)
                return $"{((long)data2).Divide()}";
            else if (value is double data3)
                return $"{((long)data3).Divide()}";
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
