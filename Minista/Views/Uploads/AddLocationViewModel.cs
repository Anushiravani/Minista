using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Minista.Views.Uploads;
using InstagramApiSharp;
using static Helper;
using Windows.UI.Core;

namespace Minista.Views.Uploads
{
    public class AddLocationViewModel : BaseModel
    {
        public ObservableCollection<InstaPlaceShort> PlaceSearches { get; set; } = new ObservableCollection<InstaPlaceShort>();

        public string PlaceSearch = string.Empty;
        bool MorePlaceAvailable = true, IsPlaceLoading = false;
        PaginationParameters PlacePagination = PaginationParameters.MaxPagesToLoad(1);
        public ScrollViewer PlaceScrollViewer;
        public void SetPlaceScrollViewer(ListView lv)
        {
            if (PlaceScrollViewer != null)
                return;
            PlaceScrollViewer = lv.FindScrollViewer();
            if (PlaceScrollViewer != null)
                PlaceScrollViewer.ViewChanging += PlaceScrollViewerViewChanging;
        }
        public async void SearchPlaces(string text = null)
        {
            try
            {
                if (text.Contains("@"))
                    text = text.Replace("@", "");
                if (text.Contains("#"))
                    text = text.Replace("#", "");
                text = text.ToLower();
                if (text.Length == 0) return;
            }
            catch { }
            var has = PlaceSearch != text;
            try
            {
                if (has)
                {
                    MorePlaceAvailable = true;

                    PlaceSearches.Clear();
                    PlacePagination = PaginationParameters.MaxPagesToLoad(1);
                }
                PlaceSearch = text;
                if (!MorePlaceAvailable)
                    return;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (has)
                        AddLocationView.Current?.ShowTopLoadingPlace();
                    else
                         AddLocationView.Current?.ShowBottomLoadingPlace();
                    var result = await InstaApi.LocationProcessor.SearchPlacesAsync(text, PlacePagination);
                    if (result.Succeeded)
                    {
                        if (result.Value.Items.Count > 0)
                        {
                            var lst = new List<InstaPlaceShort>();
                            result.Value.Items.ForEach(x => lst.Add(x.Location));

                            PlaceSearches.AddRange(lst);
                        }
                        MorePlaceAvailable = result.Value.HasMore;
                    }


                    if (has)
                         AddLocationView.Current?.HideTopLoadingPlace();
                    else
                         AddLocationView.Current?.HideBottomLoadingPlace();
                });
            }
            catch
            {
                if (has)
                     AddLocationView.Current?.HideTopLoadingPlace();
                else
                     AddLocationView.Current?.HideBottomLoadingPlace();
            }
            IsPlaceLoading = false;
        }
        private void PlaceScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (PlaceSearches == null)
                    return;
                if (!PlaceSearches.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsPlaceLoading == false)
                {
                    IsPlaceLoading = true;
                    SearchPlaces(PlaceSearch);
                }
            }
            catch (Exception ex) { ex.PrintException("PlaceScrollViewerViewChanging"); }
        }
    }
}
