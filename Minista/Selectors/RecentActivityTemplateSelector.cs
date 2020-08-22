using InstagramApiSharp.Classes.Models;
using Minista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Minista.Selectors
{
    public class RecentActivityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotSupportedTemplate { get; set; }
        public DataTemplate FriendRequestTemplate { get; set; }
        public DataTemplate MediaTaggedYouTemplate { get; set; }
        public DataTemplate FollowYouTemplate { get; set; }
        public DataTemplate LikedYouTemplate { get; set; }
        public DataTemplate CommentTemplate { get; set; }
        public DataTemplate SharedPostTemplate { get; set; }
        public DataTemplate LikedTaggedTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }



        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var Item = item as RecentActivityFeed;
            switch(Item.StoryType)
            {
                case InstagramApiSharp.Enums.InstaActivityFeedStoryType.FriendRequest:
                    return FriendRequestTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.TaggedYou:
                //    return MediaTaggedYouTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.Follow:
                //    return FollowYouTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.Like:
                //    //return LikedYouTemplate;
                //    return DefaultTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.Comment:
                //    return CommentTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.SharedPost:
                //    return SharedPostTemplate;
                case InstagramApiSharp.Enums.InstaActivityFeedStoryType.LikedTagged:
                    return LikedTaggedTemplate;
                //case InstagramApiSharp.Enums.InstaActivityFeedStoryType.FollowPeople:
                //    return LikedYouTemplate;

                default:
                    //return NotSupportedTemplate;
                    return DefaultTemplate;
            }
        }
    }
}
