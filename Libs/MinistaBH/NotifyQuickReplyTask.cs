using InstagramApiSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
namespace MinistaBH
{
    public sealed class NotifyQuickReplyTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            if (details == null)
            {
                //BackgroundTaskStorage.PutError("TriggerDetails was not ToastNotificationActionTriggerDetail.");
                return;
            }

            string arguments = details.Argument;
            if (arguments == "dismiss=True") return;

            System.Diagnostics.Debug.WriteLine(arguments);
            var f = details.UserInput?.FirstOrDefault();
            if (f == null) return;
            System.Diagnostics.Debug.WriteLine(arguments);
            System.Diagnostics.Debug.WriteLine(f);
            //NotificationHelper.ShowNotify(f, "http://musicbax.ir/wp-content/uploads/2020/04/mohsen-ebrahimzadeh-gandomi.jpg", arguments);

            var aaa = HttpUtility.ParseQueryString(new Uri("https://alaki.com/" + arguments));





        }
    }

}
