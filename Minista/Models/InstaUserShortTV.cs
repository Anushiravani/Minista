using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.Models
{
    public class InstaUserShortTV : InstaUserShort, INotifyPropertyChanged
    {
        private InstaFriendshipShortStatus _status;
        public InstaFriendshipShortStatus Friendship
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("Friendship"); }
        }
    }
}
