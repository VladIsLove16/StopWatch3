﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        //Активность
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
            }
        }
        //Имя
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        [XmlIgnore]
        public bool IsItemSelected
        {
            get { return isItemSelected; }
            set
            {
                isItemSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsItemSelected)));
            }
        }
        //Время
        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
                Console.WriteLine("ElapsedTime Changed");
            }
        }

        //создание экземпляра класса
        public StopwatchItem(string name)
        {
            IsItemSelected = false;
            Name = name;
            stopwatch = new Stopwatch();
        }
        public StopwatchItem()
        { stopwatch = new Stopwatch(); IsItemSelected = false; }
        //запуск таймера
        public void Start()
        {
            stopwatch.Start();
            IsActive = true;
        }
        //остановка таймера
        public void Stop()
        {
            stopwatch.Stop();
            IsActive = false;
        }
        public void Reset()
        {
            stopwatch.Reset();
        }
        //изменение времени, если таймер активен
        public void UpdateElapsedTime()
        {
            if (IsActive)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
            }
        }
    }
}
