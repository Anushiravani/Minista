using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.Models
{
    public class RecentActivityFeedCollection : ObservableCollection<RecentActivityFeedList>
    {
        public RecentActivityFeedList CurrentList;
        public void AddWithColumns(RecentActivityFeed feed)
        {
            if (CurrentList == null)
            {
                if (feed.StoryType == InstagramApiSharp.Enums.InstaActivityFeedStoryType.FriendRequest)
                    feed.ActivityCategoryType = RecentActivityFeedType.Unknown;

                CurrentList = new RecentActivityFeedList { ActivityCategoryType = feed.ActivityCategoryType };
                Add(CurrentList);
            }

            if (feed.ActivityCategoryType == CurrentList.ActivityCategoryType)
                CurrentList.Items.Add(feed);
            else
            {
                CurrentList = new RecentActivityFeedList { ActivityCategoryType = feed.ActivityCategoryType };
                Add(CurrentList);
                CurrentList.Items.Add(feed);
            }
        }
    }
    public class RecentActivityFeedList : INotifyPropertyChanged
    {
        public ObservableCollection<RecentActivityFeed> Items { get; set; } = new ObservableCollection<RecentActivityFeed>();

        private RecentActivityFeedType _activityCategoryType = RecentActivityFeedType.Today;
        public RecentActivityFeedType ActivityCategoryType
        {
            get { return _activityCategoryType; }
            set
            {
                _activityCategoryType = value;
                OnPropertyChanged("ActivityCategoryType");
                switch (value)
                {
                    case RecentActivityFeedType.Earlier:
                        ActivityCategoryName = "Earlier";
                        break;
                    case RecentActivityFeedType.ThisMonth:
                        ActivityCategoryName = "This Month";
                        break;
                    case RecentActivityFeedType.ThisWeek:
                        ActivityCategoryName = "This Week";
                        break;
                    case RecentActivityFeedType.Today:
                        ActivityCategoryName = "Today";
                        break;
                    case RecentActivityFeedType.Unknown:
                        ActivityCategoryName = "";
                        break;
                }
            }
        }

        private string _activityCategoryName = "";
        public string ActivityCategoryName { get { return _activityCategoryName; } set { _activityCategoryName = value; OnPropertyChanged("ActivityCategoryName"); } }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
    public class RecentActivityFeed : InstaRecentActivityFeed
    {
        public RecentActivityFeedType ActivityCategoryType { get; set; } = RecentActivityFeedType.Unknown;

    }

    public enum RecentActivityFeedType
    {
        Today,
        ThisWeek,
        ThisMonth,
        Earlier,
        Unknown = -1
    }
}
