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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Minista.Views.Uploads
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddLocationView : Page
    {
        public static AddLocationView Current;
        public AddLocationView()
        {
            this.InitializeComponent();
            Current = this;
        }

        private void PlacesLVLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AddLocationVM.SetPlaceScrollViewer(PlacesLV);
            }
            catch { }
        } 

        private void PlacesLVItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem != null && e.ClickedItem is InstaPlaceShort place)
                {
                    if (Helpers.NavigationService.Frame.Content is UploadView view && view != null)
                    {
                        view.SetLocation(new InstaLocationShort
                        {
                            Address = place.Address,
                            Name = place.Name,
                            ExternalId = place.FacebookPlacesId.ToString(),
                            ExternalSource = place.ExternalSource,
                            Lat = place.Lat,
                            Lng = place.Lng
                        });
                        Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch { }
        }

        private void SearchTextTextChanged(object sender, TextChangedEventArgs e) => DoSearch();

        private void SearchTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
                 DoSearch();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => DoSearch();

        void DoSearch()
        {
            if (string.IsNullOrEmpty(SearchText.Text))
            {
                SearchText.Focus(FocusState.Keyboard);
                return;
            }
            AddLocationVM.SearchPlaces(SearchText.Text);
        }

        #region LOADINGS Place
        public void ShowTopLoadingPlace() => TopLoadingPlace.Start();
        public void HideTopLoadingPlace() => TopLoadingPlace.Stop();


        public void ShowBottomLoadingPlace() => BottomLoadingPlace.Start();
        public void HideBottomLoadingPlace() => BottomLoadingPlace.Stop();
        #endregion LOADINGS Place
    }
}
