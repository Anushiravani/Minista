﻿using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista
{
    static class UserHelper
    {
        public static InstaBanyanSuggestions BanyanSuggestions;
        public static async Task GetBanyanAsync()
        {
            try
            {
                //await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                //{
                    var banyan = await Helper.InstaApi.GetBanyanSuggestionsAsync();
                    if (banyan.Succeeded)
                    {
                        if (banyan.Value?.Threads?.Count > 0 || banyan.Value?.Users?.Count > 0)
                            BanyanSuggestions = banyan.Value;
                    }
                //});
            }
            catch { }
        }
        public static async void GetBanyan()
        {
            try
            {
                var cur = MainPage.Current;
                var api = Helper.InstaApi;
                await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var banyan = await Helper.InstaApi.GetBanyanSuggestionsAsync();
                    if (banyan.Succeeded)
                    {
                        if (banyan.Value?.Threads?.Count > 0 || banyan.Value?.Users?.Count > 0)
                            BanyanSuggestions = banyan.Value;
                    }
                });
            }
            catch (Exception ex){ ex.PrintException("EEEXX"); }
        }
        public static async Task<InstaUserInfo> GetSelfUserAsync()
        {
            try
            {
                var user = await Helper.InstaApi.UserProcessor.GetUserInfoByIdAsync(Helper.InstaApi.GetLoggedUser().LoggedInUser.Pk);
                if (user.Succeeded)
                {
                    Helper.CurrentUser = user.Value;
                    Helper.InstaApi.UpdateUser(user.Value.ToUserShort());
                    SessionHelper.SaveCurrentSession();
                }
            }
            catch { }
            return Helper.CurrentUser;
        }
        public static async void GetSelfUser()
        {
            try
            {
                await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var user = await Helper.InstaApi.UserProcessor.GetUserInfoByIdAsync(Helper.InstaApi.GetLoggedUser().LoggedInUser.Pk);
                    if (user.Succeeded)
                    {
                        Helper.CurrentUser = user.Value;
                        Helper.InstaApi.UpdateUser(user.Value.ToUserShort());
                        SessionHelper.SaveCurrentSession();
                    }
                });
            }
            catch { }
        }
    }
}
