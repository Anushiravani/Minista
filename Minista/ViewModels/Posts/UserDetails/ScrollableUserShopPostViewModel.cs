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
    public class ScrollableUserShopPostViewModel : BaseModel
    {
        public InstaUserShort User { get; set; }
        public UserDetailsShopMediasGenerator MediaGeneratror { get; private set; } = new UserDetailsShopMediasGenerator();

        public void ResetCache()
        {
            try
            {
                try
                {
                    MediaGeneratror = null;
                }
                catch { }
                User = null;
            }
            catch { }
        }
        public async void SetInfo(InstaUserShort user, UserDetailsShopMediasGenerator generator, ScrollViewer scroll)
        {
            if (user == null) return;
            if (generator == null) return;

            User = user;
            MediaGeneratror = generator;
            await Task.Delay(500);
            if (scroll != null)
                scroll.ViewChanging += MediaGeneratror.Scroll_ViewChanging;
        }
    }
}
