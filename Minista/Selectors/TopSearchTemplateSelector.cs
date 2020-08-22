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
    public class TopSearchTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TopUserTemplate { get; set; }
        public DataTemplate TopHashtagTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var Item = item as InstaDiscoverRecentSearchesItem;
            if (Item.IsHashtag)
                return TopHashtagTemplate;
            else
                return TopUserTemplate;
        }
    }
}
