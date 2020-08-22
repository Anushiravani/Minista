using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class UserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaUser data && data != null)
            {
                var name = data.FullName;
                if (!string.IsNullOrEmpty(data.FollowersCountByLine))
                    name += $" {Helper.MiddleDot} {data.FollowersCountByLine}";
                if(data.FriendshipStatus!= null )
                    if(data.FriendshipStatus.Following)
                        name += $" {Helper.MiddleDot} Following";

                return name;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
