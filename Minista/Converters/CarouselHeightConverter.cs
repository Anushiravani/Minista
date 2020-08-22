using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;


namespace Minista.Converters
{
    class CarouselHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 380;
            if (value is InstaCarousel data && data != null)
            {
                var height = 0;
                try
                {
                    foreach (var item in data)
                    {
                        if (item.MediaType == InstaMediaType.Image)
                        {
                            if (height < item.Images[1].Height)
                                height = item.Images[1].Height;
                        }
                        else
                        {
                            if (height < item.Videos[1].Height)
                                height = item.Videos[1].Height;
                        }
                    }
                    return height;
                }
                catch { }
            }
            return 380;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
