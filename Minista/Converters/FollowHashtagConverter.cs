using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;
namespace Minista.Converters
{
    class FollowHashtagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "Follow";
            if (value is int i)
                value = System.Convert.ToBoolean(i);

            if (value is bool data)
            {
                if (data)
                    return "Unfollow";
                else
                    return "Follow";
            }
            return "Follow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
