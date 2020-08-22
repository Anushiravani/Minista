using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinistaEditorStudio.Props
{
    public class Property : PropertyBase
    {
        private string _title;
        private double _value, _minimum, _maximum;
        public string Title { get => _title; set { _title = value; OnPropertyChanged("Title"); } }
        public double Value { get => _value; set { _value = value; OnPropertyChanged("Value"); } }
        public double Minimum { get => _minimum; set { _minimum = value; OnPropertyChanged("Minimum"); } }
        public double Maximum { get => _maximum; set { _maximum = value; OnPropertyChanged("Maximum"); } }
    }
    public class PropertyGroup : PropertyBase
    {
        private string _title;
        public string Title { get => _title; set { _title = value; OnPropertyChanged("Title"); } }
        public PropertyCollection Properties { get; private set; } = new PropertyCollection();
    }
    public class PropertyCollection : ObservableCollection<Property> { }
    public class PropertyGroupCollection : ObservableCollection<PropertyGroup> { } 
    public class PropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged(string memberName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
    }
}
