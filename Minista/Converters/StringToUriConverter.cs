using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string s))
                return null;

            if (Uri.TryCreate(s, UriKind.Absolute, out Uri uri))
                return uri;

            if (Uri.TryCreate(s, UriKind.Relative, out uri))
                return uri;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Uri uri = value as Uri;
            if (uri == null)
                return null;

            return uri.OriginalString;
        }
    }
}
