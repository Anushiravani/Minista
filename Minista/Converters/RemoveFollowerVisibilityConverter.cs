using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;


namespace Minista.Converters
{
    class RemoveFollowerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is long)
            {
                if (Views.Infos.FollowView.Current != null)
                    if(Views.Infos.FollowView.Current.FollowVM.User.Pk == Helper.CurrentUser.Pk)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
