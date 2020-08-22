using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Minista.Selectors
{
    public class MediaUserInfoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate CarouselTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var Item = item as InstaMedia;
            switch(Item.MediaType)
            {
                case InstaMediaType.Carousel:
                    return CarouselTemplate;
            }
            return ImageTemplate;
        }
    }
}
