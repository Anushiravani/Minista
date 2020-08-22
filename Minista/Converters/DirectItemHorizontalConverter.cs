using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;

namespace Minista.Converters
{
    public class DirectItemHorizontalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return HorizontalAlignment.Left;
            if (value is InstaDirectInboxItem data && data != null)
            {
                if (data.ItemType == InstaDirectThreadItemType.VideoCallEvent)
                    return HorizontalAlignment.Center;
                return data.UserId == Helper.InstaApi.GetLoggedUser().LoggedInUser.Pk ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }
            else
                return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
