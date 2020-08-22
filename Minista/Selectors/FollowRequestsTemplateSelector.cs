using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InstagramApiSharp.Enums;
using Minista.ViewModels.Infos;

namespace Minista.Selectors
{
    public class FollowRequestsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate SuggestionTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var user = item as InstaUserShortFriendship;
            if(user != null)
                return UserTemplate;
            else
                return SuggestionTemplate;
            //var Item = item as FollowRequestsWithCategory;
            //if (Item.Title.ToLower().Contains("suggest"))
            //    return SuggestionTemplate;
            //else
            //    return UserTemplate;
        }
    }
}
