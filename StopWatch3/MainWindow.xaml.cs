using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using LiveCharts;
using LiveCharts.Configurations;
using System.Collections.Generic;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
namespace StopwatchApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<StopwatchItem> stopwatches;
        private DispatcherTimer timer = new DispatcherTimer();
       
            
        DateTime today = new DateTime();
        string Name1="4234";
       


//событие СВОЙСТВО ИЗМЕНЕНО
public event PropertyChangedEventHandler PropertyChanged;
        DateTime selectedDate
            {
                get { return selectedDate; }
                set
                {
                    //OnPropertyChanged("selectedDate");
                }
            }
//Массива Секундомеров
        public ObservableCollection<StopwatchItem> Stopwatches
        {
           
            get { return stopwatches; }
            set
            {
                stopwatches = value;
               // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stopwatches)));
            }
        }
        /*
        public class Date : INotifyPropertyChanged
        {
            private DateTime selectedDate;
            private DateTime today;

            public string SelectedTimeText
            {
                get { return selectedDate.ToString("d"); }
                set
                {
                    if (selectedDate != value)
                    {
                        selectedDate = value;
                        OnPropertyChanged(nameof(selectedDate));
                    }
                }
            }
       
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
         */
        public class Statistic
        {
            private Dictionary<DateTime, Dictionary<string, TimeSpan>> Global = new Dictionary<DateTime, Dictionary<string, TimeSpan>>();
            
            
            public void AddElapsedTime(DateTime day, string name, TimeSpan elapsedTime)
            {
                if (!Global.ContainsKey(day))
                    Global.Add(day, new Dictionary<string, TimeSpan>() );
                if (!Global[day].ContainsKey(name))
                    Global[day].Add(name, elapsedTime);
                else
                Global[day][name] += elapsedTime;

            }
            public Dictionary<string, TimeSpan> GetStatFor(DateTime day)
            {
                if (!Global.ContainsKey(day))
                    Global.Add(day, new Dictionary<string, TimeSpan>());
                return Global[day];
            }
            public TimeSpan GetDayTime(DateTime day)
            {
                TimeSpan daystat=TimeSpan.Zero;
                foreach (var pair_ in GetStatFor(day) )
                    daystat += pair_.Value;
                return daystat;
            }
        }
        Statistic Global = new Statistic();
        //При запуске проекта
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;
            /* Массив*/
            Stopwatches = new ObservableCollection<StopwatchItem>();
            StopwatchItem stopwatch = new StopwatchItem("First");
            Stopwatches.Add(stopwatch);
            stopwatchList.ItemsSource = stopwatches;

            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();

            selectedDate = DateTime.Today;
            today = DateTime.Today;
            Statistic Global1 = new Statistic();
            calendar.SelectedDate = DateTime.Today;
            PieSeries serie1 = new PieSeries();
            SeriesCollection collection = new SeriesCollection
            {
                new PieSeries { Values = new ChartValues<ObservableValue> { new ObservableValue(8) } },
                new PieSeries { Values = new ChartValues<ObservableValue> { new ObservableValue(4) } },
                new PieSeries {Title="a", Values = new ChartValues<ObservableValue> { new ObservableValue(2) } }
            };
            MyPieChart.Series = collection ;
        }
      
        //По клику кнопки Добавить
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            StopwatchItem stopwatch = new StopwatchItem(name);
            Stopwatches.Add(stopwatch);
            stopwatchList.ItemsSource = stopwatches;
            nameTextBox.Text = "";
        }
        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)calendar.SelectedDate;
            DateTextBlock.Text = selectedDate.ToString("d");
            DayStat.Text =Global.GetDayTime(selectedDate).ToString(@"hh\:mm\:ss");

            SeriesCollection collection = new SeriesCollection();
            foreach (var pair_ in Global.GetStatFor(selectedDate))
                collection.Add(new PieSeries
                {
                    Title = pair_.Key,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(pair_.Value.Seconds) }

                }) ;
            MyPieChart.Series = collection;
        }
        public void OpenCalendar(object sender, RoutedEventArgs e)
        {
            calendar.IsEnabled = true;
        }
            public void CloseCalendar(object sender, RoutedEventArgs e)
        {
            calendar.IsEnabled = false;
        }
       
        //По клику кнопки Старт
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;//переменная типа кнопка становится равна sender преобразованный в кнопку
            StopwatchItem stopwatch = (StopwatchItem)button.DataContext;//переменная типа секундомеритем становится равна контексту кнопки преобразованному в секундомеритем
            stopwatch.Start();
        }
//По клику кнопки Стоп
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StopwatchItem stopwatch = (StopwatchItem)button.DataContext;
            stopwatch.Stop();
            DateTime selected = (DateTime)calendar.SelectedDate;
            Global.AddElapsedTime(selected, stopwatch.Name, stopwatch.ElapsedTime);

            Console.WriteLine($"Статистика за {selected.ToString("d")}:");
            foreach (var pair_ in Global.GetStatFor(selected))
                Console.WriteLine($"{pair_.Key} {pair_.Value.ToString(@"hh\:mm\:ss")} "  );

            stopwatch.Reset();
        }
        //При тике таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (StopwatchItem stopwatch in Stopwatches)
            {
                stopwatch.UpdateElapsedTime();
                //if stopwatch.active invoke elapsed time
            }
        }

        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
    //Интерфейс модели
    public class StopwatchItem : INotifyPropertyChanged
    {
        private string name;
        private Stopwatch stopwatch;
        private bool isActive;
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
            private set
            {
                name = value;
              //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
//Время
        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
            private set {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
                Console.WriteLine("ElapsedTime Changed");
            }
        }

//создание экземпляра класса
     public StopwatchItem(string name)
        {
            Name = name;
            stopwatch = new Stopwatch();
        }  
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