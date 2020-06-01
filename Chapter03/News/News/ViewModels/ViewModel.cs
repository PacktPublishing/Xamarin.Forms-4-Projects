using System;
using System.ComponentModel;
using System.Data;

namespace News.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigate Navigation { get; set; } = new Navigator();
    }
}
