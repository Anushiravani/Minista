using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Minista.Views.Settings
{
    public class NotificationsViewModel : BaseModel
    {
        public InstaNotificationSettingsSectionList Items { get; set; } = new InstaNotificationSettingsSectionList();

        public async void RunLoadMore()
        {
            await RunLoadMoreAsync();
        }
        async Task RunLoadMoreAsync()
        {
            await Helper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync();
            });
        }

        private async Task LoadMoreItemsAsync()
        {
            try
            {
                // show loadings
                // get notifications settings!
                var result = await Helper.InstaApi.AccountProcessor.GetNotificationsSettingsAsync();
                if (result.Succeeded)
                {
                    Items.Clear();
                    Items.AddRange(result.Value);
                }
            }
            catch { }
        }
    }
}
