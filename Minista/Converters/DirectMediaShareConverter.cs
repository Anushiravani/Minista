using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Minista.Converters
{
    class DirectMediaShareConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is InstaMedia data)
            {
                switch (data.MediaType)
                {
                    case InstaMediaType.Carousel:
                        {
                            if (!string.IsNullOrEmpty(data.CarouselShareChildMediaId))
                            {
                                var defaultMedia = data.Carousel.FirstOrDefault(m => m.InstaIdentifier == data.CarouselShareChildMediaId);
                                if (defaultMedia != null)
                                    return new Uri(defaultMedia.Images.FirstOrDefault().Uri);
                            }
                            return new Uri(data.Carousel.FirstOrDefault().Images.FirstOrDefault().Uri);
                        }
                    default:
                        return new Uri(data.Images.FirstOrDefault().Uri);
                }
            
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
