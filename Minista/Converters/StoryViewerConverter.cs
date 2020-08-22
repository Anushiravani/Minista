using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Minista.Converters
{
    public class StoryViewerPollVoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            if (value is InstaStoryVoterItem data && data != null)
            {
                if ((int)data.Vote == 0)
                    return data.VoteYesText.ToLower();
                else
                    return data.VoteNoText.ToLower();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class StorySliderVoteDoubleConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 0;
            if (value is double data)
                return data * 10;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class StoryColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new SolidColorBrush(Colors.White);
            if (value is string data && data != null)
                return ("#ff" + data.Replace("#", "")).GetColorBrush();
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class StoryColorGradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new SolidColorBrush(Colors.White);
            if (value is string data && data != null)
            {
                var spl = data.Split('|');
                var backgroundGradient = new LinearGradientBrush();
                var startBG = new GradientStop { Color = ("#ff" + spl[0].Replace("#", "")).GetColorFromHex() };
                var endBG = new GradientStop { Color = ("#ff" + spl[1].Replace("#", "")).GetColorFromHex(), Offset = 1 };
                backgroundGradient.GradientStops.Add(startBG);
                backgroundGradient.GradientStops.Add(endBG);
                return backgroundGradient;
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
