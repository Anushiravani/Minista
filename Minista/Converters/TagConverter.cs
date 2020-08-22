using System;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class TagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaHashtag data && data != null)
            {
                var name = data.FormattedMediaCount.ToLower() + " posts";
                if (!string.IsNullOrEmpty(data.SearchResultSubtitle))
                    if(data.SearchResultSubtitle.ToLower().Contains("followed"))
                    name += $" {Helper.MiddleDot} {data.SearchResultSubtitle}";

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
