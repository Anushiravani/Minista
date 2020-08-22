using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    public class FollowingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is InstaFriendshipShortStatus data && data != null)
                return Visibility.Visible;
            else if (value is InstaUserShortFriendship data2 && data2 != null)
                return Visibility.Visible;
            else if (value is InstaFriendshipStatus data3 && data3 != null)
                return Visibility.Visible;
            else if (value is InstaStoryFriendshipStatus data4 && data4 != null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

