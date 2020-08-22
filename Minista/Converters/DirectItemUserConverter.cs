using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Minista.ViewModels.Direct;
using Minista.Views.Direct;

namespace Minista.Converters
{
    public class DirectItemUserProfilePictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return Helper.NoProfilePictureUri;
            if (value is InstaDirectInboxItem data && data != null)
            {
                try
                {
                    if (data.UserId != Helper.CurrentUser.Pk)
                    {
                        if (ThreadView.Current != null)
                        {
                            var user = ThreadView.Current.ThreadVM.CurrentThread.Users.FirstOrDefault(x => x.Pk == data.UserId);
                            if (user != null)
                                return user.ProfilePicture.ToUri();
                        }
                        else if(DirectRequestsThreadView.Current != null)
                        {
                            var user = DirectRequestsThreadView.Current.Thread.Users.FirstOrDefault(x => x.Pk == data.UserId);
                            if (user != null)
                                return user.ProfilePicture.ToUri();
                        }
                    }
                }
                catch { }
            }
            return Helper.NoProfilePictureBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
