using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InstagramApiSharp.Enums;
namespace Minista.Selectors
{
    //public enum InstaFeedsType
    //{
    //    Media,
    //    Story,
    //    SuggestedUsers,
    //    EndOfFeedDemarcator,
    //    Hashtag
    //}
    public class MediaMainTemplateSelector : DataTemplateSelector
    {
        //public DataTemplate VideoTemplate { get; set; }
        public DataTemplate HashtagTemplate { get; set; }
        public DataTemplate SuggestedUsersTemplate { get; set; }
        public DataTemplate EndOfFeedDemarcatorTemplate { get; set; }
        public DataTemplate MediaTemplate { get; set; }
        public DataTemplate StoryTemplate { get; set; }
        public DataTemplate UserCardsTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object sender, DependencyObject container)
        {
            var item = sender as InstaPost;
            switch(item.Type)
            {
                case InstaFeedsType.Hashtag:
                    return HashtagTemplate;

                case InstaFeedsType.SuggestedUsers:
                    return SuggestedUsersTemplate;

                case InstaFeedsType.EndOfFeedDemarcator:
                    return EndOfFeedDemarcatorTemplate;

                default:
                case InstaFeedsType.Media:
                    return MediaTemplate;

                case InstaFeedsType.StoriesNetego:
                    return StoryTemplate;

                case InstaFeedsType.SuggestedUsersCard:
                    return UserCardsTemplate;

            }
        }
    }
    //public class MediaMainTemplateSelector : DataTemplateSelector
    //{
    //    public DataTemplate VideoTemplate { get; set; }
    //    public DataTemplate ImageTemplate { get; set; }
    //    public DataTemplate HashtagTemplate { get; set; }
    //    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    //    {
    //        var Item = item as InstaCarouselItem;
    //        if (Item.MediaType == InstaMediaType.Image)
    //            return ImageTemplate;
    //        else
    //            return VideoTemplate;
    //    }
    //}
}
