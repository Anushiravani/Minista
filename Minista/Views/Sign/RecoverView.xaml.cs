using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060	
using static Helper;
namespace Minista.Views.Sign
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecoverView : Page
    {
        public RecoverView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
                GetType().RemovePageFromBackStack();
        }
        private void UsernameTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                HandleUsername();
        }
        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            HandleUsername();
        }
        InstaUserLookup UserLookup;
        async void HandleUsername()
        {
            try
            {
                if (string.IsNullOrEmpty(UsernameText.Text))
                    UsernameText.Focus(FocusState.Keyboard);
                else
                {
                    if (InstaApiTrash == null)
                        InstaApiTrash = BuildApi();

                    var userLookup = await InstaApiTrash.GetRecoveryOptionsAsync(UsernameText.Text.Trim().ToLower());
                    if (userLookup.Succeeded)
                    {
                        var resp = userLookup.Value;
                        UserLookup = resp;
                        if (resp.LookupSourceType == InstagramApiSharp.Enums.InstaLookupType.Username &&
                            resp.User != null)
                        {

                            UserPicture.Fill = resp.User.ProfilePicture.GetImageBrush();
                            UserText.Text = resp.User.UserName.ToUpper();
                            UserGrid.Visibility = Visibility.Visible;
                        }
                        if (resp.CanEmailReset)
                            EmailButton.Visibility = Visibility.Visible;
                        if (resp.CanSmsReset)
                            SMSButton.Visibility = Visibility.Visible;

                        First.Visibility = Visibility.Collapsed;
                        Second.Visibility = Visibility.Visible;

                    }
                    else
                        userLookup.Info.Message.ShowErr();
                }
            }
            catch { }
        }

        public void Reset()
        {
            UserGrid.Visibility= EmailButton.Visibility = SMSButton.Visibility = Second.Visibility = Visibility.Collapsed;
            First.Visibility = Visibility.Visible;
        }
        private async void EmailButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var em = await InstaApiTrash.SendRecoveryByEmailAsync(UsernameText.Text.Trim().ToLower());
                if (em.Succeeded)
                    em.Value.Body.ShowMsg("Success");
                else
                    em.Info.Message.ShowErr();
            }
            catch { }
        }

        private async void SMSButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ph = await InstaApiTrash.SendRecoveryByPhoneAsync(UsernameText.Text.Trim().ToLower());
                if (ph.Succeeded)
                    ph.Value.Body.ShowMsg(ph.Value.Title);
                else
                    ph.Info.Message.ShowErr();
            }
            catch { }
        }
    }
}
