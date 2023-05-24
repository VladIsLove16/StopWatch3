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

namespace StopwatchApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<StopwatchItem> stopwatches;
        private DispatcherTimer timer = new DispatcherTimer();
        public event PropertyChangedEventHandler PropertyChanged;
        string statpath = "stat.xml";
        string listpath = "list.xml";
        Statistic Global;
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
        public class Statistic
        {
            private Dictionary<DateTime, Dictionary<string, TimeSpan>> Global;
            private TimeSpan selectedDayTime;
            private DateTime selectedDay;
            public Statistic(){
                Global = new Dictionary<DateTime, Dictionary<string, TimeSpan>>();
            }
            public Dictionary<DateTime, Dictionary<string, TimeSpan>> Global_
            {
                get { return Global; }
               set { Global = value;
                    OnPropertyChanged(nameof(Global_));
                }
            }
            public TimeSpan SelectedDayTime
            {
                get { return selectedDayTime; }
                set
                {
                    selectedDayTime = value;
                    OnPropertyChanged(nameof(SelectedDayTime));
                }
            }
            public DateTime SelectedDay 
            {
                get { return selectedDay ; }
                set
                {
                    selectedDay  = value;
                    OnPropertyChanged(nameof(SelectedDay ));
                }
            }
            public void AddElapsedTime(DateTime day, string name, TimeSpan elapsedTime)
            {
                if (!Global.ContainsKey(day))
                    Global.Add(day, new Dictionary<string, TimeSpan>() );
                if (!Global[day].ContainsKey(name))
                    Global[day].Add(name, elapsedTime);
                else
                    Global[day][name] += elapsedTime;
                if (day == SelectedDay)
                    SelectedDayTime += elapsedTime;
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
                int total=0;
                foreach (var pair_ in GetStatFor(day) )
                    total +=(int) pair_.Value.TotalSeconds;
                daystat = TimeSpan.FromSeconds( total);
                return daystat;
            }
           
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)calendar.SelectedDate;
            Global.SelectedDay = selectedDate;
            DateTextBlock.Text = selectedDate.ToString("d");
            DayStat.Text = Global.GetDayTime(selectedDate).ToString(@"hh\:mm\:ss");

            SeriesCollection collection = new SeriesCollection();
            foreach (var pair_ in Global.GetStatFor(selectedDate))
                collection.Add(new PieSeries
                {
                    Title = pair_.Key,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(pair_.Value.Seconds) }

                });
            MyPieChart.Series = collection;
        }
        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tab1.IsSelected)
            {
                Console.WriteLine("Tab1.IsSelected");
            }

            if (Tab2.IsSelected)
            {
                SeriesCollection collection = new SeriesCollection();
                foreach (var pair_ in Global.GetStatFor(Global.SelectedDay))
                    collection.Add(new PieSeries
                    {
                        Title = pair_.Key,
                        Values = new ChartValues<ObservableValue> { new ObservableValue(pair_.Value.Seconds) }

                    });
                MyPieChart.Series = collection;
                DayStat.Text = Global.GetDayTime((DateTime)calendar.SelectedDate).ToString(@"hh\:mm\:ss");
                Console.WriteLine("Tab2.IsSelected");
            }
            if (Tab3.IsSelected)
                Console.WriteLine("Tab3.IsSelected");
        }
        public static class DataSaver
        {
            public static void  SaveStat(Dictionary<DateTime, Dictionary<string, TimeSpan>> data, string filePath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<DateTime, Dictionary<string, TimeSpan>>));
                Console.WriteLine("Serialer created");
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                   
                    serializer.Serialize(stream, data); Console.WriteLine("Serialer Serialized");
                }
            }
            public static void SaveList(ObservableCollection<StopwatchItem> data, string filePath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<StopwatchItem>));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }
            }
        }
        // Загрузка пользовательского типа из файла
        public static T LoadCustomTypeFromFile<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(object));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
        public static Dictionary<DateTime, Dictionary<string, TimeSpan>> LoadStat(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<DateTime, Dictionary<string, TimeSpan>>));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (Dictionary<DateTime, Dictionary<string, TimeSpan>>)serializer.Deserialize(stream);
            }
        }
        public static ObservableCollection<StopwatchItem> LoadList(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<StopwatchItem>));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (ObservableCollection < StopwatchItem >)serializer.Deserialize(stream);
            }
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
                // Отображаем диалоговое окно с вопросом о сохранении изменений
                MessageBoxResult result = MessageBox.Show("Хотите сохранить изменения?", "Сохранить изменения", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Выполняем сохранение изменений
                try
                {
                    DataSaver.SaveStat(Global.Global_, statpath);
                    DataSaver.SaveList(stopwatches, listpath);
                    Console.WriteLine("Сохранение удалось");
                }
                catch
                {
                    Console.WriteLine("Сохранение не удалось");
                   
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                // Отменяем закрытие приложения
                e.Cancel = true;
            }
        }
        // Использование сохранения и загрузки пользовательского типа


        // Сохранение пользовательского типа в файл

        // Загрузка пользовательского типа из файла
        //При запуске проекта
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;
            /* Массив*/
            if (!File.Exists(statpath) || !File.Exists(listpath))
            {
                Stopwatches = new ObservableCollection<StopwatchItem>();
                StopwatchItem stopwatch = new StopwatchItem("First");
                Stopwatches.Add(stopwatch);
            }
            else
            {
                try
                {
                    Console.WriteLine("Загрузка Статистики");
                    Global= new Statistic();
                    Global.Global_ = LoadStat(statpath); //new Statistic();
                    Global.AddElapsedTime(DateTime.Today, "test", TimeSpan.FromSeconds(3));
                    Console.WriteLine("Удачная загрузка Статистики");
                }
                catch { 
                    Console.WriteLine("Неудачная загрузка Статистики");
                    Global = new Statistic();
                    Global.AddElapsedTime(DateTime.Today, "test", TimeSpan.FromSeconds(1));
                }
                try {
                    Console.WriteLine("Загрузка списка секундомеров");
                    Stopwatches = LoadList(listpath);
                    Console.WriteLine("Удачная загрузка списка секундомеров");
                }
                catch {
                    Console.WriteLine("Неудачная загрузка списка секундомеров");
                    stopwatchList.ItemsSource = stopwatches;
                    SeriesCollection collection = new SeriesCollection
                     {
                    new PieSeries { Values = new ChartValues<ObservableValue> { new ObservableValue(1) },Title="Default" },
                     };
                    MyPieChart.Series = collection;
                }
            }
            stopwatchList.ItemsSource = stopwatches;
            Console.WriteLine("Выставление Интервала");
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();
            calendar.SelectedDate = DateTime.Today;
            Closing += MainWindow_Closing;
            //Statistic loadedObject = LoadCustomTypeFromFile(filePath);
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
            Global.AddElapsedTime(DateTime.Today, stopwatch.Name, stopwatch.ElapsedTime);
            Console.WriteLine($"Статистика за {DateTime.Today.ToString("d")}:");
            foreach (var pair_ in Global.GetStatFor(DateTime.Today))
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
            set
            {
                name = value;
              //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
//Время
        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
            set {
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
        public StopwatchItem()
        { stopwatch = new Stopwatch(); }
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