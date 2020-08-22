using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class AspectRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 460;
            if (value is int i)
            {
                if (i > 720)
                    return (i * 2);
                else
                    return i;
            }
            if (value is InstaImage img)
            {
                if(img.Height > img.Width)
                    return (img.Height * 2);
                else
                    return (img.Width * 2);
            }
            if (value is InstaVideo vid)
            {
                if (vid.Height > vid.Width)
                    return (vid.Height * 2);
                else
                    return (vid.Width * 2);
            }
            return 460;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
