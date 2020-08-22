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
    public class MediaCarouselTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate HashtagTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var Item = item as InstaCarouselItem;
            if (Item.MediaType == InstaMediaType.Image)
                return ImageTemplate;
            else
                return VideoTemplate;
        }
    }
}
