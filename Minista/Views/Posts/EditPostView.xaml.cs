using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace Minista.Views.Posts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPostView : Page
    {
        public InstaMedia Media;
        bool IsPageLoaded = false;
        bool IsChangedSomething = false;
        bool CanChangeSomething = false;
        public EditPostView()
        {
            InitializeComponent();
            Loaded += EditPostViewLoaded;
        }

        private async void EditPostViewLoaded(object sender, RoutedEventArgs e)
        {
            if (IsPageLoaded) return;

            IsPageLoaded = false;
            if (Media != null)
            {
                try
                {
                    CanChangeSomething = false;
                    if (Media.MediaType == InstaMediaType.Carousel)
                        ImageView.Source = Media.Carousel[0].Images[0].Uri.GetBitmap();
                    else
                        ImageView.Source = Media.Images[0].Uri.GetBitmap();
                    CaptionText.Text = Media.Caption?.Text;
                    await Task.Delay(350);
                    CanChangeSomething = true;
                }
                catch { }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is InstaMedia media)
                Media = media;
        }
        private void CaptionTextTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CanChangeSomething)
            {


                IsChangedSomething = true;
            }
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsChangedSomething)
            {
                // do something!
            }
            Helpers.NavigationService.GoBack();
        }

        private async void OkButtonClick(object sender, RoutedEventArgs e)
        {
            var tags = new List<InstaUserTagUpload>();
            if (Media.UserTags?.Count > 0)
            {
                foreach(var item in Media.UserTags)
                {
                    tags.Add(new InstaUserTagUpload
                    {
                        Pk = item.User.Pk,
                        X = item.Position.X,
                        Y = item.Position.Y,
                        Username = item.User.UserName
                    });
                }
            }
            var editPost = await Helper.InstaApi.MediaProcessor.EditMediaAsync(Media.InstaIdentifier, CaptionText.Text, Media.Location, tags.ToArray());
            if (editPost.Succeeded)
            {
                Helper.ShowNotify("Your post edited successfully.");
                Helpers.NavigationService.GoBack();
            }
            else
                Helper.ShowNotify("ERR: " + editPost.Info.Message);

        }

        private void ImageView_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
