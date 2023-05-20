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
namespace StopwatchApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<StopwatchItem> stopwatches;
        private DispatcherTimer timer = new DispatcherTimer();
        DateTime selectedDate=new DateTime();

        //событие СВОЙСТВО ИЗМЕНЕНО
        public event PropertyChangedEventHandler PropertyChanged;
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
//При запуске проекта
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
/* Массив*/ Stopwatches = new ObservableCollection<StopwatchItem>();
            StopwatchItem stopwatch = new StopwatchItem("First");
            Stopwatches.Add(stopwatch);
            stopwatchList.ItemsSource = stopwatches;
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();
            selectedDate = DateTime.Today;
            DateTextBlock.Text = selectedDate.ToString();
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
            private set { }
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