using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace StopwatchApp
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private DateTime _selectedDate=DateTime.Today;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
