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

namespace Minista.UserControls.Story
{
    public sealed partial class StoryPollUc : UserControl
    {
        public InstaStoryItem StoryItem { get; private set; }
        public InstaStoryPollItem PollItem { get; private set; }
        public StoryPollUc()
        {
            this.InitializeComponent();
        }
        public void SetItem(InstaStoryPollItem poll, InstaStoryItem storyItem)
        {
            if (poll == null) return;
            if (storyItem == null) return;
            PollItem = poll;
            StoryItem = storyItem;
            try
            {
                // age viewer vote -1 nabod
                if (poll.PollSticker?.Tallies?.Count > 0)
                {
                    YesButton.Content = txtYes.Text = poll.PollSticker.Tallies[0].Text;
                    NoButton.Content = txtNo.Text = poll.PollSticker.Tallies[1].Text;
                    CalculateYesNo();
                }

            }
            catch { }
        }
        void CalculateYesNo()
        {
            try
            {
                if (PollItem.PollSticker.ViewerVote != -1 || PollItem.PollSticker.Finished /*|| PollItem.PollSticker.ViewerCanVote*/)
                {
                    var yes = PollItem.PollSticker.Tallies[0].Count;
                    var no = PollItem.PollSticker.Tallies[1].Count;
                    double count = yes + no;

                    var yesPercent = (int)Math.Round((yes * 100) / count);
                    var noPercent = (int)Math.Round((no * 100) / count);
                    txtYesPercent.Text = $"{yesPercent}%";
                    txtNoPercent.Text = $"{noPercent}%";
                    if (yes == 0)
                    {
                        Column1.Width = new GridLength(0, GridUnitType.Pixel);
                        Column2.Width = new GridLength(0, GridUnitType.Pixel);
                        Column3.Width = new GridLength(1, GridUnitType.Star);
                        txtNoPercent.Visibility = txtNo.Visibility = Visibility.Visible;
                        txtYesPercent.Visibility = txtYesPercent.Visibility = Visibility.Collapsed;
          
                    }
                    else if (no == 0)
                    {
                        Column1.Width = new GridLength(1, GridUnitType.Star);
                        Column2.Width = new GridLength(0, GridUnitType.Pixel);
                        Column3.Width = new GridLength(0, GridUnitType.Pixel);
                        txtYesPercent.Visibility = txtYesPercent.Visibility = Visibility.Visible;
                        txtNoPercent.Visibility = txtNo.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Column1.Width = new GridLength(1, GridUnitType.Star);
                        Column2.Width = new GridLength(1, GridUnitType.Auto);
                        Column3.Width = new GridLength(1, GridUnitType.Star);
                        txtYesPercent.Visibility = txtYesPercent.Visibility = Visibility.Visible;
                        txtNoPercent.Visibility = txtNo.Visibility = Visibility.Visible;
                    }
                    FirstGrid.Visibility = Visibility.Collapsed;
                    SecondGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    FirstGrid.Visibility = Visibility.Visible;
                    SecondGrid.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }
        private async void YesButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    PollItem.PollSticker.ViewerVote = 0;
                    PollItem.PollSticker.Tallies[0].Count++;
                    CalculateYesNo();
                    var result = await Helper.InstaApi.StoryProcessor
                    .VoteStoryPollAsync(StoryItem.Id, PollItem.PollSticker.PollId.ToString(), InstaStoryPollVoteType.Yes);
                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }
        }

        private async void NoButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    PollItem.PollSticker.ViewerVote = 1;
                    PollItem.PollSticker.Tallies[1].Count++;
                    CalculateYesNo();
                    var result = await Helper.InstaApi.StoryProcessor
                    .VoteStoryPollAsync(StoryItem.Id, PollItem.PollSticker.PollId.ToString(), InstaStoryPollVoteType.No);

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
    }
}
