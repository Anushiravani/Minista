using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
namespace Minista.Converters
{
    public class PostNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaMedia data && data != null)
            {
                if (data.FollowHashtagInfo != null)
                    return "#" + data.FollowHashtagInfo.Name.ToLower();
                else
                    return data.User.UserName.ToLower();
            }
            if (value is InstaReelFeed data2 && data2 != null)
                return data2.User.UserName.ToLower();
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class PostLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaMedia data && data != null)
            {
                if (data.Location != null)
                {
                    if (data.FollowHashtagInfo != null)
                        return $"{data.User.UserName.ToLower()} {Helper.MiddleDot} {data.Location.Name}";
                    else
                        return data.Location.Name;
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class PostPictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaMedia data && data != null)
            {
                if (data.FollowHashtagInfo != null)
                    return new Uri(data.FollowHashtagInfo.ProfilePicture);
                else
                    return new Uri(data.User.ProfilePicture);
            }
            if (value is InstaReelFeed data2 && data2 != null)
                return new Uri(data2.User.ProfilePicture);
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class PostHashtagIconVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is InstaMedia data && data != null)
            {
                if (data.FollowHashtagInfo != null)
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
