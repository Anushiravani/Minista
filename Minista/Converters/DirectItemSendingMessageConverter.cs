using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Minista.ViewModels.Direct;
using Minista.Views.Direct;
using InstagramApiSharp.Enums;
using System.ComponentModel.DataAnnotations;

namespace Minista.Converters
{
    public class DirectItemSendingMessageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Visibility.Collapsed;
            if (value is InstaDirectInboxItem data && data != null)
            {
                try
                {
                    if (data.UserId != Helper.CurrentUser.Pk)
                        return Visibility.Collapsed;
                    else return Visibility.Visible;
                }
                catch { }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DirectItemSendingMessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultColor = "#FF858588".GetColorBrush();
            if (value == null) return defaultColor;

            try
            {
                if (value is InstaDirectInboxItemSendingType data)
                {
                    switch (data)
                    {
                        case InstaDirectInboxItemSendingType.Seen:
                            return "#FF0079CE".GetColorBrush(); // abi
                        case InstaDirectInboxItemSendingType.Sent:
                            return defaultColor;
                        case InstaDirectInboxItemSendingType.Pending:
                        default:
                            return "#FF595959".GetColorBrush();
                    }
                }
            }
            catch { }
            return defaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DirectItemSendingMessageTikTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var def = Helper.OneTikMaterialIcon;
            if (value == null) return def;

            try
            {
                if (value is InstaDirectInboxItemSendingType data)
                {
                    switch (data)
                    {
                        case InstaDirectInboxItemSendingType.Seen:
                            return Helper.DoubleTikMaterialIcon;
                        case InstaDirectInboxItemSendingType.Sent:
                            return def;
                        case InstaDirectInboxItemSendingType.Pending:
                        default:
                            return Helper.PendingTikMaterialIcon;
                    }
                }
            }
            catch { }
            return def;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
