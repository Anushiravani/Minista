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
using InstagramApiSharp.Helpers;
namespace Minista.UserControls.Story
{
    public sealed partial class StoryCountdownUc : UserControl
    {
        readonly DispatcherTimer Timer = new DispatcherTimer();
        private TimeSpan CountdownTime;
        public InstaStoryItem StoryItem { get; private set; }

        public InstaStoryCountdownItem CountdownItem { get; private set; }
        public StoryCountdownUc()
        {
            InitializeComponent();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += TimerTick;
            Tapped += StoryCountdownUcTapped;
        }

        private async void StoryCountdownUcTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                SetHolding(true);
                Helper.ShowNotify($"\"{CountdownItem.CountdownSticker.Text}\"\r\n{CountdownItem.CountdownSticker.EndTime.ToString()}", 3000);

                await System.Threading.Tasks.Task.Delay(4000);
                SetHolding(false);
            }
            catch { }
        }

        public void SetItem(InstaStoryCountdownItem countdownItem, InstaStoryItem storyItem)
        {
            if (countdownItem == null) return;
            if (storyItem == null) return;
            CountdownItem = countdownItem;
            StoryItem = storyItem;
            try
            {
                if (countdownItem.CountdownSticker != null)
                {
                    var backgroundGradient = new LinearGradientBrush();
                    var startBG = new GradientStop { Color = ("#ff" + countdownItem.CountdownSticker.StartBackgroundColor.Replace("#", "")).GetColorFromHex() };
                    var endBG = new GradientStop { Color = ("#ff" + countdownItem.CountdownSticker.EndBackgroundColor.Replace("#", "")).GetColorFromHex(), Offset = 1 };
                    backgroundGradient.GradientStops.Add(startBG);
                    backgroundGradient.GradientStops.Add(endBG);
                    MainGrid.Background = backgroundGradient;
                    txtText.Text = countdownItem.CountdownSticker.Text;
                    txtText.Foreground = countdownItem.CountdownSticker.TextColor.GetColorBrush();
                    HoursText.Foreground = MinutesText.Foreground = SecondsText.Foreground
                        = countdownItem.CountdownSticker.DigitColor.GetColorBrush();

                    HoursGrid.Background = MinutesGrid.Background = SecondsGrid.Background = countdownItem.CountdownSticker.DigitCardColor.GetColorBrush();

                    CountdownTime = DateTime.UtcNow - countdownItem.CountdownSticker.EndTime;
                }
            }
            catch { }
        }


        private void TimerTick(object sender, object e)
        {
            try
            {
                if (CountdownTime.Seconds == 0 && CountdownTime.Hours == 0 && CountdownTime.Minutes == 0)
                {
                    SetTime();
                    Timer.Stop();
                }
                else
                {
                    CountdownTime = CountdownTime.Add(TimeSpan.FromSeconds(-1));
                    SetTime();
                }
            }
            catch { }
        }
        void SetTime()
        {
            try
            {
                HoursText.Text = CountdownTime.Hours.ToString("00");
                MinutesText.Text = CountdownTime.Minutes.ToString("00");
                SecondsText.Text = CountdownTime.Seconds.ToString("00");
            }
            catch { }
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
