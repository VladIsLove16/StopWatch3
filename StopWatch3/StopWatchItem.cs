using System;
using System.ComponentModel;
using System.Diagnostics;
namespace StopWatchItem
{
    [Serializable]
    public class StopwatchItem : INotifyPropertyChanged
    {
        private string name;
        private Stopwatch stopwatch;
        private bool isActive;
        private bool isItemSelected;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }
        public bool IsItemSelected
        {
            get { return isItemSelected; }
            set
            {
                isItemSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsItemSelected)));
            }
        }
        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
                Console.WriteLine("ElapsedTime Changed");
            }
        }
        public StopwatchItem(string name)
        {
            IsItemSelected = false;
            Name = name;
            stopwatch = new Stopwatch();
        }
        public StopwatchItem()
        { stopwatch = new Stopwatch(); IsItemSelected = false; }
        public void Start()
        {
            stopwatch.Start();
            IsActive = true;
        }
        public void Stop()
        {
            stopwatch.Stop();
            IsActive = false;
        }
        public void Reset()
        {
            stopwatch.Reset();
        }
        public void UpdateElapsedTime()
        {
            if (IsActive)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
            }
        }
    }
}
