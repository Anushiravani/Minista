using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class BlockedDateTimeConverter : IValueConverter
    {
        public object Convert(object value)
        {
            return Convert(value, value.GetType(), null, string.Empty);
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = System.Convert.ToDateTime(value);
            return $"Blocked at {date.ToString("MMM", CultureInfo.InvariantCulture)} {date.Day}, {date.ToString("yyyy", CultureInfo.InvariantCulture)} - {date.ToString("hh:mm tt")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
