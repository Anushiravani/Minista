using System.ComponentModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace Minista.Controls
{
    public sealed class RunNew : Inline, INotifyPropertyChanged
    {
        object _tag;
        string _text = null;
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public RunNew() { }

        public object Tag { get { return _tag; } set { _tag = value; OnPropertyChanged("Tag"); } }

        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }

        public FlowDirection FlowDirection { get; set; }

        public static DependencyProperty FlowDirectionProperty = DependencyProperty.Register(
        "FlowDirection",
        typeof(FlowDirection),
        typeof(RunNew),
        new PropertyMetadata(FlowDirection.LeftToRight));

    }
}
