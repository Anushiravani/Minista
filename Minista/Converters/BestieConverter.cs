using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
   public class BestieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "Add";
            if (value is InstaUserShort data)
            {
                if (data.IsBestie)
                    return "Remove";
            }
            if (value is bool flag)
            {
                if (flag)
                    return "Remove";
            }
            return "Add";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
