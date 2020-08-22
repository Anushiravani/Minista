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
    public class ThreadItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotSupportedTemplate { get; set; }
        public DataTemplate LikeTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate MediaShareTemplate { get; set; }
        public DataTemplate MediaTemplate { get; set; }
        public DataTemplate PlaceholderTemplate { get; set; }
        public DataTemplate NoTemplate { get; set; }
        public DataTemplate StoryShareTemplate { get; set; }
        public DataTemplate ProfileTemplate { get; set; }
        public DataTemplate FelixShareTemplate { get; set; }
        public DataTemplate RavenMediaTemplate { get; set; }
        public DataTemplate LocationMediaTemplate { get; set; }
        public DataTemplate ReelShareMediaTemplate { get; set; }
        public DataTemplate HashtagMediaTemplate { get; set; }
        public DataTemplate VoiceMediaTemplate { get; set; }
        public DataTemplate LinkMediaTemplate { get; set; }
        public DataTemplate LiveViewerInviteTemplate { get; set; }
        public DataTemplate VideoCallEventTemplate { get; set; }
        public DataTemplate AnimatedMediaTemplate { get; set; }
        public DataTemplate ActionLogTemplate { get; set; }
        // Empties
        public DataTemplate EmptyStoryShareTemplate { get; set; }
        public DataTemplate EmptyReelShareMediaTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var Item = item as InstaDirectInboxItem;


            if (Item.ItemType == InstaDirectThreadItemType.StoryShare && Item.StoryShare != null && Item.StoryShare?.Media == null)
                return EmptyStoryShareTemplate;

            if (Item.ItemType == InstaDirectThreadItemType.ReelShare && Item.ReelShareMedia != null && Item.ReelShareMedia?.Media != null)
            {
                try
                {
                    if (Item.ReelShareMedia.Media.Images.Count == 0 && Item.ReelShareMedia.Media.Videos.Count == 0)
                        return EmptyReelShareMediaTemplate;
                }
                catch { return EmptyReelShareMediaTemplate; }
            }
            switch (Item.ItemType)
            {
                case InstaDirectThreadItemType.MediaShare:
                    return MediaShareTemplate;
                case InstaDirectThreadItemType.Media:
                    return MediaTemplate;
                case InstaDirectThreadItemType.Text:
                    return TextTemplate;
                case InstaDirectThreadItemType.Like:
                    return LikeTemplate;
                case InstaDirectThreadItemType.Placeholder:
                    return PlaceholderTemplate;
                case InstaDirectThreadItemType.StoryShare:
                    return StoryShareTemplate;
                case InstaDirectThreadItemType.Profile:
                    return ProfileTemplate;
                case InstaDirectThreadItemType.FelixShare:
                    return FelixShareTemplate;
                case InstaDirectThreadItemType.RavenMedia:
                    return RavenMediaTemplate;
                case InstaDirectThreadItemType.Location:
                    return LocationMediaTemplate;
                case InstaDirectThreadItemType.ReelShare:
                    return ReelShareMediaTemplate;
                case InstaDirectThreadItemType.Hashtag:
                    return HashtagMediaTemplate;
                case InstaDirectThreadItemType.VoiceMedia:
                    return VoiceMediaTemplate;
                case InstaDirectThreadItemType.Link:
                    return LinkMediaTemplate;
                case InstaDirectThreadItemType.LiveViewerInvite:
                    return LiveViewerInviteTemplate;
                case InstaDirectThreadItemType.VideoCallEvent:
                    return VideoCallEventTemplate;
                case InstaDirectThreadItemType.AnimatedMedia:
                    return AnimatedMediaTemplate;
                case InstaDirectThreadItemType.ActionLog:
                    return ActionLogTemplate;
                default:
                    return NotSupportedTemplate;
            }
        }
    }
}
