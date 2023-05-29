using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using StopWatchItem;
namespace StopwatchApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<StopwatchItem> stopwatches;
        private DispatcherTimer timer = new DispatcherTimer();
        public event PropertyChangedEventHandler PropertyChanged;
        string TrackerPath = "tracker.xml";
        string stopwatchesPath = "stopwatches.xml";
        TaskTracker Tracker;
        public void UpdateChart()
        {
            SeriesCollection collection = new SeriesCollection();
            List<Task> dailyTasks= Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
            foreach (var task in dailyTasks)
                collection.Add(new PieSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(task.Time.TotalSeconds) }

                });
            MyPieChart.Series = collection;
        }
        
        public ObservableCollection<StopwatchItem> Stopwatches
        {
            get { return stopwatches; }
            set
            {
                stopwatches = value;
                {
                    Console.WriteLine("Список изменён");
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stopwatches)));
                   
                }
            }
        }
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)calendar.SelectedDate;
            DateTextBlock.Text = selectedDate.ToString("d");
            DayStat.Text=Tracker.CalculateTotalTimeForDate(selectedDate).ToString(@"hh\:mm\:ss");
            UpdateChart();
        }
        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tab1.IsSelected)
            {
                Console.WriteLine("Tab1.IsSelected");
            }

            if (Tab2.IsSelected)
            {
                 
                DateTime selectedDate = (DateTime)calendar.SelectedDate;
                UpdateChart();
                DateTextBlock.Text = selectedDate.ToString("d");
                DataContext = Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
                DayStat.Text = Tracker.CalculateTotalTimeForDate(selectedDate).ToString(@"hh\:mm\:ss");
                Console.WriteLine("Tab2.IsSelected");
            }
            if (Tab3.IsSelected)
                Console.WriteLine("Tab3.IsSelected");
        }
        public static T LoadCustomTypeFromFile<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(object));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
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
        public void OpenCalendar(object sender, RoutedEventArgs e)
        {
            calendar.IsEnabled = true;
        }
            public void CloseCalendar(object sender, RoutedEventArgs e)
        {
            calendar.IsEnabled = false;
        }
        private void DeleteStopWatch(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;//переменная типа кнопка становится равна sender преобразованный в кнопку
            StopwatchItem stopwatch = (StopwatchItem)button.DataContext;//переменная типа секундомеритем становится равна контексту кнопки преобразованному в секундомеритем
            stopwatches.Remove(stopwatch);
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
            Console.WriteLine("Обновление времени на {0}", stopwatch.ElapsedTime.TotalSeconds);
            Tracker.UpdateTaskTimeForDate(DateTime.Today, stopwatch.Name, stopwatch.ElapsedTime);
            Console.WriteLine($"Статистика за {DateTime.Today.ToString("d")}:");
            foreach (var task in Tracker.GetTasksByDate(DateTime.Today))
                Console.WriteLine($"{task.Name} {task.Time.ToString(@"hh\:mm\:ss")} "  );
            stopwatch.Reset();
        }
        //При тике таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (StopwatchItem stopwatch in Stopwatches)
            {
                stopwatch.UpdateElapsedTime();
            }
        }

        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {
            if (1 == 2)
            {
                    SeriesCollection collection = new SeriesCollection
                {
                    new PieSeries { Values = new ChartValues<ObservableValue> { new ObservableValue(8) } },
                    new PieSeries { Values = new ChartValues<ObservableValue> { new ObservableValue(4) } },
                    new PieSeries {Title="a", Values = new ChartValues<ObservableValue> { new ObservableValue(2) } }
                };
                
                    MyPieChart.Series = collection;
            }
        }
        private void NextDate(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = calendar.SelectedDate ?? DateTime.Today;
            DateTime nextDate = selectedDate.AddDays(1);
            calendar.SelectedDate = nextDate;
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Хотите сохранить изменения?", "Сохранить изменения", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Serializer.SerializeTaskTracker(Tracker, TrackerPath);
                    Serializer.SerializeList(Stopwatches,stopwatchesPath);
                }
                catch (Exception ex){ Console.WriteLine("Сериализация отменена");
                    Console.WriteLine(ex);
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Tracker = new TaskTracker();
            if (!File.Exists(stopwatchesPath))
            {
                Stopwatches = new ObservableCollection<StopwatchItem>();
                StopwatchItem stopwatch = new StopwatchItem("First");
                Stopwatches.Add(stopwatch);
            }
            else try
                {
                    Stopwatches =Serializer.DeserializeList(stopwatchesPath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

            if (!File.Exists(TrackerPath))
            {
                Tracker.AddTask(DateTime.Today, new Task("Default", TimeSpan.FromSeconds(1)));
            }
            else try
                {
                    Tracker = Serializer.DeserializeTaskTracker(TrackerPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            calendar.SelectedDate = DateTime.Today;
            stopwatchList.ItemsSource = Stopwatches;
            Console.WriteLine("Выставление Интервала");
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();
            Closing += MainWindow_Closing;
            //Statistic loadedObject = LoadCustomTypeFromFile(filePath);
        }

        private void IsNull(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Tracker.GetTasksByDate((DateTime)calendar.SelectedDate).Count);
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}