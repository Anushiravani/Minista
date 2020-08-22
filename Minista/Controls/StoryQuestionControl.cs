using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Minista.Controls
{
    public class StoryQuestionControl : Grid
    {
        public string StoryUsername { get; set; }
        public string StoryId { get; set; }
        public InstaStoryQuestionItem StoryQuestionItem { get; set; }
    }
}
