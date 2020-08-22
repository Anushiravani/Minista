using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InstagramApiSharp.Enums;

namespace Minista.UserControls.Story.StoryViews
{
    public class StoryViewerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PollResultsTemplate { get; set; }
        public DataTemplate PollVotersTemplate { get; set; }
        public DataTemplate QuestionsResponsesTemplate { get; set; }
        public DataTemplate QuizResultsTemplate { get; set; }
        public DataTemplate QuizAnswersTemplate { get; set; }
        public DataTemplate SliderResultsTemplate { get; set; }
        public DataTemplate SliderAnswersTemplate { get; set; }
        public DataTemplate ViewersTemplate { get; set; }
        public DataTemplate ViewersNotAvailableTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object sender, DependencyObject container)
        {
            var item = sender as StoryViewObject;
            switch (item.Type)
            {
                case StoryViewObjectType.PollResults:
                    return PollResultsTemplate;
                case StoryViewObjectType.PollVoters:
                    return PollVotersTemplate;
                case StoryViewObjectType.QuestionsResponses:
                    return QuestionsResponsesTemplate;
                case StoryViewObjectType.QuizResults:
                    return QuizResultsTemplate;
                case StoryViewObjectType.QuizAnswers:
                    return QuizAnswersTemplate;
                case StoryViewObjectType.SliderResults:
                    return SliderResultsTemplate;
                case StoryViewObjectType.SliderAnswers:
                    return SliderAnswersTemplate;
                case StoryViewObjectType.ViewersNotAvailable:
                    return ViewersNotAvailableTemplate;
                case StoryViewObjectType.Viewers:
                default:
                    return ViewersTemplate;
            }
        }
    }
}
