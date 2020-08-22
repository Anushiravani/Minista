using Minista.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Minista.Views.Security
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasscodeView : Page
    {
        public event OnSuccessPassHandler OnSuccessPass;
        public PasscodeView()
        {
            this.InitializeComponent();

        }
        public async void Load()
        {
            try
            {
                if (await KeyCredentialManager.IsSupportedAsync())
                {
                    var result = await KeyCredentialManager.OpenAsync(Helper.AppName);
                    if (result.Credential != null)
                    {
                        var signResult = await result.Credential.RequestSignAsync(CryptographicBuffer.ConvertStringToBinary(DeviceHelper.PackageName, BinaryStringEncoding.Utf8));
                        if (signResult.Status == KeyCredentialStatus.Success)
                        {
                            Unlock();
                        }
                        else
                        {
                            BiometricsButton.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        var creationResult = await KeyCredentialManager.RequestCreateAsync(Helper.AppName, KeyCredentialCreationOption.ReplaceExisting);
                        if (creationResult.Status == KeyCredentialStatus.Success)
                        {
                            Unlock();
                        }
                        else
                        {
                            BiometricsButton.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    PassText.Focus(FocusState.Keyboard);
                }
            }
            catch { }
        }
        private void PassTextPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassText.Password.Length == 4)
            {
                if (PassText.Password.All(x => x >= '0' && x <= '9') && Helper.Passcode.Check(PassText.Password))
                {
                    Unlock();
                }
                else
                {
                    VisualUtilities.ShakeView(PassText);
                    PassText.Password = string.Empty;
                }
            }
        }

        private void PassTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                CheckPass();
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            CheckPass();
        }

        private async void BiometricsButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await KeyCredentialManager.IsSupportedAsync())
                {
                    var creationResult = await KeyCredentialManager.RequestCreateAsync(Helper.AppName, KeyCredentialCreationOption.ReplaceExisting);
                    if (creationResult.Status == KeyCredentialStatus.Success)
                    {
                        Unlock();
                    }
                    else
                    {
                        BiometricsButton.Visibility = Visibility.Visible;
                    }
                }
            }
            catch { }
        }
        private void CheckPass()
        {
            //if (_passcodeService.Check(PassText.Password))
            //{
            //    Unlock();
            //}
            //else
            //{
            //    VisualUtilities.ShakeView(PassText);
            //    PassText.Password = string.Empty;
            //}
        }
        private void Unlock()
        {
            //_passcodeService.Unlock();
            Accepted = true;
            PassText.Password = "";
            Visibility = Visibility.Collapsed;
            OnSuccessPass?.Invoke();
        }
        public bool Accepted { get; private set; } = true;
    }
    public delegate void OnSuccessPassHandler();
    internal interface IPasscode
    {
        bool IsEnabled { get; }
        bool IsLocked { get; }
        bool IsBiometricsEnabled { get; set; }

        void Lock();
        void Unlock();

        bool Check(string passcode);
        void Set(string passcode);
        void Reset();
    }
    public class Passcode : IPasscode
    {
        private Passcode() { }
        private string Password { get; set; }
        public bool IsEnabled => ApplicationSettingsHelper.LoadSettingsValue("MINISTAVERSION") != null;

        public bool IsLocked { get; private set; }

        public bool IsBiometricsEnabled { get; set; }

        public bool Check(string passcode)
        {
            try
            {
                var dec = Dec(Password);
                return dec.Equals(passcode);
            }
            catch { }
            return false;
        }

        public void Lock()
        {
            if (MainPage.Current?.PassCodeView != null && MainPage.Current?.LockControl != null)
            {
                MainPage.Current.PassCodeView.Visibility = Visibility.Visible;
                MainPage.Current.LockControl.Visibility = Visibility.Visible;
            }
        }

        public void Reset()
        {
            ApplicationSettingsHelper.RemoveSettingsValue("MINISTAVERSION");

            MainPage.Current.LockControl.Visibility = Visibility.Collapsed;
        }

        public void Set(string passcode)
        {
            Password = Enc(passcode);

            MainPage.Current.LockControl.Visibility = Visibility.Visible;
            ApplicationSettingsHelper.SaveSettingsValue("MINISTAVERSION", Password);
        }

        public void Unlock()
        {
            if (MainPage.Current?.PassCodeView != null)
                MainPage.Current.PassCodeView.Visibility = Visibility.Collapsed;
        }
        string Enc(string pass) => CryptoHelper.Encrypt(pass, CryptoHelper.Key2);
        string Dec(string pass) => CryptoHelper.Decrypt(pass, CryptoHelper.Key2);

        private void Load()
        {
            try
            {
                var obj = ApplicationSettingsHelper.LoadSettingsValue("MINISTAVERSION");
                if (obj != null)
                {
                    Password = Convert.ToString(obj);
                    IsLocked = true;
                }
            }
            catch { }
        }
        internal static IPasscode Build()
        {
            var p = new Passcode();
            p.Load();
            return p;
        }
    }
}
