using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    public class FollowingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "Follow";
            value.GetType().PrintDebug();
            if (value is InstaFriendshipShortStatus data && data != null)
            {
                if (data.Following)
                    return "Unfollow";
                else if (data.OutgoingRequest)
                    return "Requested";
                else
                    return "Follow";
            }
            else if (value is InstaUserShortFriendship data2 && data2 != null)
            {
                var status = data2.FriendshipStatus;
                if (status.Following)
                    return "Unfollow";
                else if (status.OutgoingRequest)
                    return "Requested";
                else
                    return "Follow";
            }
            else if (value is InstaFriendshipStatus data3 && data3 != null)
            {
                var status = data3;
                if (status.Following)
                    return "Unfollow";
                else if (status.OutgoingRequest)
                    return "Requested";
                else
                    return "Follow";
            }
            else if (value is InstaStoryFriendshipStatus data4 && data4 != null)
            {
                var status = data4;
                if (status.Blocking)
                    return "Unblock";
                else if(status.FollowedBy && !status.Following)
                    return "Follow Back";
                else if (status.Following)
                    return "Unfollow";
                else if (status.OutgoingRequest)
                    return "Requested";
                else
                    return "Follow";
            }
            else if (value is bool data5)
            {
                if (data5)
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


    public class NonFollowerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "Unfollow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
