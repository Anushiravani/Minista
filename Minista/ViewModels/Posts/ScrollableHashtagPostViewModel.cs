using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Minista.ItemsGenerators;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
#pragma warning disable IDE0044	
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
#pragma warning disable CS0618	
namespace Minista.ViewModels.Posts
{
    public class ScrollableHashtagPostViewModel : BaseModel
    {
        public InstaHashtag Hashtag { get; private set; }
        public HashtagsRecentGenerator HashtagsRecentGenerator { get; private set; } = new HashtagsRecentGenerator();
        public HashtagsTopGenerator HashtagsTopGenerator { get; set; } = new HashtagsTopGenerator();

        public void ResetCache()
        {
            try
            {
                try
                {
                    HashtagsRecentGenerator = null;
                }
                catch { }
                try
                {
                    HashtagsTopGenerator = null;
                }
                catch { }
                Hashtag = null;
            }
            catch { }
        }
        public async void SetInfo(InstaHashtag hashtag, HashtagsRecentGenerator generator, ScrollViewer scroll)
        {
            if (hashtag == null) return;
            if (generator == null) return;

            Hashtag = hashtag;
            try
            {
                HashtagsTopGenerator = null;
            }
            catch { }
            HashtagsRecentGenerator = generator;
            await Task.Delay(500);
            if (scroll != null)
                scroll.ViewChanging += HashtagsRecentGenerator.ScrollViewChanging;
        }
        public async void SetInfo(InstaHashtag hashtag, HashtagsTopGenerator generator, ScrollViewer scroll)
        {
            if (hashtag == null) return;
            if (generator == null) return;

            Hashtag = hashtag;
            try
            {
                HashtagsRecentGenerator = null;
            }
            catch { }
            HashtagsTopGenerator = generator;
            await Task.Delay(500);
            if (scroll != null)
                scroll.ViewChanging += HashtagsTopGenerator.ScrollViewChanging;
        }
    }
}
