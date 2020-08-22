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
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Windows.UI.Xaml.Shapes;

namespace Minista.UserControls.Story
{
    public sealed partial class StorySliderUc : UserControl
    {
        TextBlock SliderText;
        Ellipse EllipseImage, Ellipse2;
        Grid CoverSliderGrid;
        public InstaStoryItem StoryItem { get; private set; }
        public InstaStorySliderItem SliderItem { get; private set; }
        public StorySliderUc()
        {
            InitializeComponent();
            PointerEntered += StorySliderUcPointerEntered;
            PointerReleased += StorySliderUcPointerReleased;

            var pointerCaptureLostHandler = new PointerEventHandler(OnSliderCaptureLost);
            XLider.AddHandler(PointerCaptureLostEvent, pointerCaptureLostHandler, true);

            var keyUpEventHandler = new KeyEventHandler(OnKeyUp);
            XLider.AddHandler(KeyUpEvent, keyUpEventHandler, true);
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs args)
        {
            Vote(); SetHolding(false);
        }

        private void OnSliderCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Vote(); SetHolding(false);
        }
        private void StorySliderUcPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            SetHolding(false);
        }

        private void StorySliderUcPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SetHolding(true);
        }

        public void SetItem(InstaStorySliderItem slider, InstaStoryItem storyItem)
        {
            if (slider == null) return;
            if (storyItem == null) return;
            SliderItem = slider;
            StoryItem = storyItem;
        }
        void SetSliderInfo()
        {
            try
            {
                if (SliderItem.SliderSticker != null)
                {
                    var bgColor = ("#ff" + SliderItem.SliderSticker.BackgroundColor.Replace("#", "")).GetColorBrush();
                    MainGrid.Background = bgColor;

                    txtQuestion.Text = SliderItem.SliderSticker.Question?.Trim().TrimEnd();
                    txtQuestion.Foreground = ("#ff" + SliderItem.SliderSticker.TextColor.Replace("#", "")).GetColorBrush();

                    XLider2.Value = SliderItem.SliderSticker.SliderVoteAverage * 10;

                    if (SliderText != null && EllipseImage != null)
                    {
                        SliderText.Text = SliderItem.SliderSticker.Emoji;
                        try
                        {
                            if(Ellipse2!= null)
                            Ellipse2.Fill = bgColor;
                        }
                        catch { }
                        if (SliderItem.SliderSticker.ViewerVote != -1)
                        {
                            DisbaleGrid.Visibility = Visibility.Visible;
                            XLider.Value = SliderItem.SliderSticker.ViewerVote * 10;
                            SliderText.Visibility = Visibility.Collapsed;
                            try
                            {
                                if (Ellipse2 != null)
                                    Ellipse2.Fill = bgColor;
                            }
                            catch { }
                            if (SliderItem.SliderSticker.ViewerVote == SliderItem.SliderSticker.SliderVoteAverage)
                            {
                                XLider2.Visibility = Visibility.Collapsed;
                                EllipseImage.Fill = Helper.CurrentUser.ProfilePicture.GetImageBrush();
                            }
                            else
                            {
                                XLider2.Visibility = Visibility.Visible;
                           
                                EllipseImage.Fill = Helper.CurrentUser.ProfilePicture.GetImageBrush();
                            }
                        }
                        else
                        {
                            SliderText.Visibility = Visibility.Visible;
                            XLider2.Visibility = Visibility.Collapsed;
                            // ghaziyash fargh dare
                        }
                    }
                }
            }
            catch { }
        }
        private void SliderTextLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                SliderText = sender as TextBlock;
                //SetSliderInfo();
            }
        }

        private void EllipseImageLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                EllipseImage = sender as Ellipse;
                SetSliderInfo();
            }
        }
        private void CoverSliderGridLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CoverSliderGrid = sender as Grid;
                CoverSliderGrid.PointerReleased += CoverSliderGridPointerReleased;
            }
        }

        private void CoverSliderGridPointerReleased(object sender, PointerRoutedEventArgs e) => Vote();
        private async void Vote()
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    SliderItem.SliderSticker.ViewerVote = (XLider.Value / 10);
                    SetSliderInfo();
                    var result = await Helper.InstaApi.StoryProcessor
                    .VoteStorySliderAsync(StoryItem.Id, SliderItem.SliderSticker.SliderId.ToString(), SliderItem.SliderSticker.ViewerVote);

                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }

        }
        void SetHolding(bool flag)
        {
            try
            {
                if (Helpers.NavigationService.Frame.Content is Views.Main.StoryView story && story != null)
                    story.IsHolding = flag;
            }
            catch { }
        }

        private void Ellipse2Loaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
                Ellipse2 = sender as Ellipse;
        }
    }
}
