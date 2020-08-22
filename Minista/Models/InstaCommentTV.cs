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
    public class InstaCommentTV : INotifyPropertyChanged
    {
        public ObservableCollection<InstaCommentTV> ChildComments = new ObservableCollection<InstaCommentTV>();

        public int Type { get; set; }

        public int BitFlags { get; set; }

        public long UserId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        private int _likesCount;
        public int LikesCount { get => _likesCount; set { _likesCount = value; OnPropertyChanged("LikesCount"); } }

        public DateTime CreatedAt { get; set; }

        public InstaContentType ContentType { get; set; }

        public InstaUserShort User { get; set; }

        public long Pk { get; set; }

        public string Text { get; set; }

        public bool DidReportAsSpam { get; set; }

        private bool _haslikedcm;
        public bool HasLikedComment { get => _haslikedcm; set { _haslikedcm = value; OnPropertyChanged("HasLikedComment"); } }

        public int ChildCommentCount { get; set; }

        public bool HasMoreTailChildComments { get; set; }

        public bool HasMoreHeadChildComments { get; set; }

        public List<InstaCommentShort> PreviewChildComments { get; set; } = new List<InstaCommentShort>();

        public List<InstaUserShort> OtherPreviewUsers { get; set; } = new List<InstaUserShort>();

        public bool Equals(InstaCommentTV comment)
        {
            return Pk == comment?.Pk;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstaCommentTV);
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
