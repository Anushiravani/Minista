using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class ThreadItemBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //#FF1D1C1C   #FF252525
            if (value == null) return "#FF1D1C1C".GetColorBrush();
            if (value is InstaDirectInboxItem data && data != null)
            {
                if(data.UserId == Helper.CurrentUser.Pk)
                    return "#FF373737".GetColorBrush();
            }
            return "#FF1D1C1C".GetColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
