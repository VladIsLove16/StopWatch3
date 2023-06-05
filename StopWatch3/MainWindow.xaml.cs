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
using System.Globalization;
using System.Windows.Input;
using System.Windows.Data;
using System.Linq;
namespace StopwatchApp
{
    public class Inverser : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        private StopwatchItem selectedStopwatch;
        private ObservableCollection<StopwatchItem> stopwatches;
        private DispatcherTimer timer = new DispatcherTimer();
        public event PropertyChangedEventHandler PropertyChanged;
        string TrackerPath = "tracker.xml";
        string stopwatchesPath = "stopwatches.xml";
        TaskTracker Tracker;
        
        public void UpdateChart(List<Task> dailyTasks)
        {
            SeriesCollection collection = new SeriesCollection();
            foreach (var task in dailyTasks)
                collection.Add(new PieSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(task.Time.TotalSeconds) }
                });
            MyPieChart.Series = collection;
        }
        public void UpdateChart()
        {
            SeriesCollection collection = new SeriesCollection();
            List<Task> dailyTasks = Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
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
                   // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stopwatches)));
                   
                }
            }
        }
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Дата изменена");
            SelectedDatesCollection dates = calendar.SelectedDates;
            DateTextBlock.Text=dates[0].ToString("d");
            if (dates.Count > 1)
            {
                DateTextBlock.Text += "-";
                DateTextBlock.Text += dates[dates.Count - 1].ToString("d");
                List < DateTime >DateList= dates.ToList();
                List<Task> tasks = Tracker.GetTasksByDates(DateList);
                DayStat.Text = Tracker.CalculateTotalTimeForTasks(tasks).ToString();
                StatiscticList.ItemsSource = tasks;
                Console.WriteLine(tasks.Count) ;
                UpdateChart(tasks);
            }
            else { 
                DayStat.Text=Tracker.CalculateTotalTimeForDate((DateTime)calendar.SelectedDate).ToString(@"hh\:mm\:ss");
                StatiscticList.ItemsSource = Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
                Console.WriteLine(Tracker.GetTasksByDate((DateTime)calendar.SelectedDate).Count);
                Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
                UpdateChart();
            }
           
           
            
        }
        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tab1.IsSelected)
            {
                Console.WriteLine("Tab1.IsSelected");
            }

            if (Tab2.IsSelected)
            {
                StatiscticList.Items.Refresh();
                DateTime selectedDate = (DateTime)calendar.SelectedDate;
                UpdateChart();
                DateTextBlock.Text = selectedDate.ToString("d");
               // DataContext = Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
                DayStat.Text = Tracker.CalculateTotalTimeForDate(selectedDate).ToString(@"hh\:mm\:ss");
                Console.WriteLine("Tab2.IsSelected");
            }
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
            stopwatches.Add(stopwatch);
            nameTextBox.Text = "";
        }
       
        public StopwatchItem SelectedStopwatch
        {
            get { return selectedStopwatch; }
            set
            {
                selectedStopwatch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStopwatch)));
            }
        }
      
        private void StopwatchesSelectionChanged(object sender, MouseButtonEventArgs e)
        {
            StopwatchItem ListSelectedItem = (StopwatchItem)stopwatchList.SelectedItem;
            Console.WriteLine(ListSelectedItem.Name);
            if (SelectedStopwatch == null)
            {
                SelectedStopwatch = ListSelectedItem;
                SelectedStopwatch.IsItemSelected = true;

            }
            else
            {
                SelectedStopwatch.IsItemSelected = false;
                if (SelectedStopwatch == ListSelectedItem)
                { 
                    SelectedStopwatch = null;
                }
                else
                {
                    SelectedStopwatch = ListSelectedItem;
                    SelectedStopwatch.IsItemSelected = true;
                }
            }
        }
        private void DeleteStopWatch(object sender, RoutedEventArgs e)
        {
            Stopwatches.Remove(SelectedStopwatch);
        }
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
                Console.WriteLine(SelectedStopwatch==null?"null":SelectedStopwatch.Name);
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
        public List<Task> TaskList=new List<Task>(); 
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
            Tab2Grid.DataContext = this;
            stopwatchList.ItemsSource = Stopwatches;
            calendar.SelectedDate = DateTime.Today;
            TaskList = Tracker.GetTasksByDate((DateTime)calendar.SelectedDate);
            StatiscticList.ItemsSource = TaskList;
           
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
            TaskList.Clear();
        }
    }

}