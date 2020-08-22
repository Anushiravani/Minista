using InstagramApiSharp.Classes.Models;
using Minista.Helpers;
using Minista.ItemsGenerators;
using Minista.Views.Infos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Minista.ViewModels.Infos
{
    public class HashtagViewModel : BaseModel
    {
        string hashtagText_;
        public string HashtagText { get { return hashtagText_; } set { hashtagText_ = value; OnPropertyChanged("HashtagText"); } }
        InstaHashtag hashtag_;
        public InstaHashtag Hashtag { get { return hashtag_; } set { hashtag_ = value; OnPropertyChanged("Hashtag"); } }
        public InstaHashtagOwner Owner { get; set; }
        public InstaReelFeed Reel { get; set; }
        public ObservableCollection<InstaStoryItem> Stories { get; set; } = new ObservableCollection<InstaStoryItem>();

        public HashtagsRecentGenerator HashtagsRecentGenerator { get; set; } = new HashtagsRecentGenerator();
        public HashtagsTopGenerator HashtagsTopGenerator { get; set; } = new HashtagsTopGenerator();
        public HashtagViewModel() => SetVM();

        void SetVM()
        {
            HashtagsRecentGenerator.SetVM(this);
            HashtagsTopGenerator.SetVM(this);
        }
        public void SetHashtag(string hashtag)
        {
            HashtagText = hashtag;
            HashtagsTopGenerator.Hashtag = hashtag;
            HashtagsRecentGenerator.Hashtag = hashtag;
            Refresh();
        }
        public void ResetCache()
        {
            HashtagText = null;
            Hashtag = null;
            Owner = null;
            Reel = null;
            Stories.Clear();
            HashtagsRecentGenerator.ResetCache();
            HashtagsTopGenerator.ResetCache();
        }

        async void GetHashtagInfo()
        {
            try
            {
                await Helper.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var h = await Helper.InstaApi.HashtagProcessor.GetHashtagInfoAsync(HashtagText);
                    if (h.Succeeded)
                        Hashtag = h.Value;
                });
            }
            catch { }
        }

        async void GetHashtagStories()
        {
            try
            {
                await Helper.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var s = await Helper.InstaApi.HashtagProcessor.GetHashtagStoriesAsync(HashtagText);
                    if (s.Succeeded)
                    {
                        Reel = s.Value.ToReel();
                        Stories.Clear();
                        Stories.AddRange(s.Value.Items);
                        Owner = s.Value.Owner;
                    }
                });
            }
            catch { }
        }


        public void Refresh()
        {
            GetHashtagInfo();
            GetHashtagStories();
            HashtagsTopGenerator.RunLoadMore(true);
        }

    }
}
