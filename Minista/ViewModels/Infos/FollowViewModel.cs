using InstagramApiSharp.Classes.Models;
using Minista.Helpers;
using Minista.ItemsGenerators;
using Minista.Views.Infos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

//following
//&order=date_followed_earliest
//&order=date_followed_late
namespace Minista.ViewModels.Infos
{
    public class FollowViewModel : BaseModel
    {
        string username_;
        public string Username { get { return username_; } set { username_ = value; OnPropertyChanged("Username"); } }

        public InstaUserShort User;

        public FollowingsGenerator FollowingsGenerator { get; set; } = new FollowingsGenerator();
        public FollowersGenerator FollowersGenerator { get; set; } = new FollowersGenerator();
        public MutualFriendsGenerator MutualFriendsGenerator { get; set; } = new MutualFriendsGenerator();

        public void SetUser(InstaUserShort user)
        {
            User = user;
            Username = user.UserName;
            FollowingsGenerator.SetUserId(user.Pk);
            FollowersGenerator.SetUserId(user.Pk);
            MutualFriendsGenerator.SetUserId(user.Pk);
        }

    }
}
