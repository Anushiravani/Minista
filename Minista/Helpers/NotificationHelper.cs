using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
namespace Minista.Helpers
{
    class NotificationHelper
    {

        public static void ShowSuccessNotify(string title, string thumbnail)
        {
            try
            {
                // And send the notification
                ToastNotificationManager.CreateToastNotifier().Show(GetSuccessNotify(title, thumbnail));
            }
            catch { }
        }
        public static ToastNotification GetSuccessNotify(string title, string thumbnail)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Media downloaded"
                                },
                                new AdaptiveText()
                                {
                                    Text = title
                                }
                            },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = thumbnail,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                //Actions = new ToastActionsCustom()
                //{
                //    Buttons =
                //        {
                //            new ToastButton("Open", "")
                //            {
                //                ActivationType = ToastActivationType.Background
                //            },
                //            new ToastButton("Close", "")
                //            {
                //                ActivationType = ToastActivationType.Foreground
                //            }
                //        }
                //},
                Launch = "action=d7"
            };

            // Create the toast notification
            return new ToastNotification(toastContent.GetXml());
        }

        public static ToastNotification GetFailedNotify(string title, string thumbnail, string text = null)
        {
            return GetSingleNotify(title, thumbnail, "Download failed");
        }

        public static void ShowNotify(string text, string thumbnail, string title = null)
        {
            try
            {
                // And send the notification
                ToastNotificationManager.CreateToastNotifier().Show(GetSingleNotify(text, thumbnail, title));
            }
            catch { }
        }
        public static ToastNotification GetSingleNotify(string text, string thumbnail, string title = null)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },
                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = thumbnail,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                }
            };

            // Create the toast notification
            return new ToastNotification(toastContent.GetXml());
        }

        public static ToastNotification GetSuccessUserNotify(string title, string thumbnail)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "User profile downloaded"
                                },
                                new AdaptiveText()
                                {
                                    Text = title
                                }
                            },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = thumbnail,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                //Actions = new ToastActionsCustom()
                //{
                //    Buttons =
                //        {
                //            new ToastButton("Open", "")
                //            {
                //                ActivationType = ToastActivationType.Background
                //            },
                //            new ToastButton("Close", "")
                //            {
                //                ActivationType = ToastActivationType.Foreground
                //            }
                //        }
                //},
                Launch = "action=d7"
            };

            // Create the toast notification
            return new ToastNotification(toastContent.GetXml());
        }
        public static void ShowToast(string text, string thumbnail, string title = null)
        {
            try
            {
                // And send the notification
                ToastNotificationManager.CreateToastNotifier().Show(GetSingleUserNotify(text, thumbnail, title));
            }
            catch { }
        }
        public static ToastNotification GetSingleUserNotify(string text, string thumbnail, string title = null)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },
                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = thumbnail,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                }
            };

            // Create the toast notification
            return new ToastNotification(toastContent.GetXml());
        }

    }
}
