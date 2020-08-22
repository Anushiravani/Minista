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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Minista.Views.TV
{
    public sealed partial class TVSearchView : UserControl
    {
        public TVSearchView()
        {
            this.InitializeComponent();
            Loaded += TVSearchView_Loaded;
        }

        private void TVSearchView_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                KeyDown -= OnKeyDownHandler;
            }
            catch { }
            KeyDown += OnKeyDownHandler;

        }
        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == Windows.System.VirtualKey.F5)
                    Search(SearchText.Text);
            }
            catch { }
        }

        private void SearchTextTextChanged(object sender, TextChangedEventArgs e)
        {
            Search(SearchText.Text);
        }

        private void SearchTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Search(SearchText.Text);
            }
        }

        async void Search(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    ShowLoading();
                    SuggestedLV.Visibility = Visibility.Collapsed;
                    SearchItemsLV.Visibility = Visibility.Collapsed;
                    await SearchVM.Suggested.RunLoadMoreAsync();
                    SuggestedLV.Visibility = Visibility.Visible;
                    SearchItemsLV.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowLoading();
                    SuggestedLV.Visibility = Visibility.Collapsed;
                    SearchItemsLV.Visibility = Visibility.Collapsed;
                    SearchVM.SearchItems.SearchWord = text;
                    await SearchVM.SearchItems.RunLoadMoreAsync();
                    SuggestedLV.Visibility = Visibility.Collapsed;
                    SearchItemsLV.Visibility = Visibility.Visible;
                }
            }
            catch { }
            try
            {
                HideLoading();
            }
            catch { }
        }

        public async void Show()
        {
            SuggestedLV.Visibility = Visibility.Visible;
            SearchItemsLV.Visibility = Visibility.Collapsed;
            Visibility = Visibility.Visible;
            try
            {
                await SearchVM.Suggested.RunLoadMoreAsync();
            }
            catch { }
        }

        public void Hide()
        {
            SearchText.Text = "";
            SuggestedLV.Visibility = Visibility.Collapsed;
            SearchItemsLV.Visibility = Visibility.Collapsed;
            Visibility = Visibility.Collapsed;
        }

        public void ShowLoading()
        {
            LoadingUC.Start();
            LoadingGrid.Visibility = Visibility.Visible;
        }

        public void HideLoading()
        {
            LoadingUC.Stop();
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Hide();
            //MainPage.Current.Footer.Show();
        } 

        private void LVItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is InstaTVSearchResult clicked && clicked != null)
            {
                try
                {
                    Hide();
                    Helper.OpenProfile(clicked);
                    //MainPage.Current.Footer.ShowUser(new InstaUserShortTV
                    //{
                    //    Pk = clicked.User.Pk,
                    //    FullName = clicked.User.FullName,
                    //    IsPrivate = clicked.User.IsPrivate,
                    //    IsVerified = clicked.User.IsVerified,
                    //    ProfilePicture = clicked.User.ProfilePicture,
                    //    ProfilePictureId = clicked.User.ProfilePictureId,
                    //    ProfilePicUrl = clicked.User.ProfilePicUrl,
                    //    UserName = clicked.User.UserName,
                    //});
                }
                catch { }
            }
        }
    }
}
