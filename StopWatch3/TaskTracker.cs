using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StopwatchApp
{

    [Serializable]
    [XmlRoot("TaskTracker")]
    public class TaskTracker
    {
        [XmlArray("TasksByDate")]
        [XmlArrayItem("DailyTasks")]
        public List<DailyTasks> TasksByDate { get; set; }

        public TaskTracker()
        {
            TasksByDate = new List<DailyTasks>();
        }

        public void AddTask(DateTime date, Task task)
        {
            DailyTasks dailyTasks = TasksByDate.Find(d => d.Date.Date == date.Date);
            if (dailyTasks == null)
            {
                dailyTasks = new DailyTasks(date.Date);
                TasksByDate.Add(dailyTasks);
            }
            dailyTasks.Tasks.Add(task);
        }

        public List<Task> GetTasksByDate(DateTime date)
        {
            DailyTasks dailyTasks = TasksByDate.Find(d => d.Date.Date == date.Date);
            Console.WriteLine("Дата существует");
            return dailyTasks?.Tasks ?? new List<Task>();
        }

        public void UpdateTaskTimeForDate(DateTime date, string taskName, TimeSpan newTime)
        {
            DailyTasks dailyTasks = TasksByDate.Find(d => d.Date.Date == date.Date);
            dailyTasks?.UpdateTaskTime(taskName, newTime);
        }
        public TimeSpan CalculateTotalTimeForDate(DateTime date)
        {
            try
            {
                TimeSpan time;
                DailyTasks dailyTasks = TasksByDate.Find(d => d.Date.Date == date.Date);
                if (dailyTasks != null)
                {
                    time = dailyTasks.CalculateTotalTime();
                }
                else
                {
                    dailyTasks = new DailyTasks(date);
                    TasksByDate.Add(dailyTasks);
                    time = TimeSpan.Zero;
                }
                return time;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return TimeSpan.Zero;
            }
            
        }

    }
    public class DailyTasks
    {
        [XmlElement("Date")]
        public DateTime Date { get; set; }
        [XmlElement("Tasks")]
        public List<Task> Tasks { get; set; }

        public DailyTasks()
        {
            Tasks = new List<Task>();
        }

        public DailyTasks(DateTime date)
        {
            Date = date;
            Tasks = new List<Task>();
        }

        public void UpdateTaskTime(string taskName, TimeSpan Time)
        {
            Task task = Tasks.Find(t => t.Name == taskName);
            if (task != null)
            {
                task.Time += Time;
            }
            else Tasks.Add(new Task(taskName, Time));

        }
        public TimeSpan CalculateTotalTime()
        {
            TimeSpan totalTime = TimeSpan.Zero;
            foreach (Task task in Tasks)
            {
                    totalTime += task.Time;
            }

            return totalTime;
        }
    }

    [Serializable]
    public class Task :INotifyPropertyChanged
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        private TimeSpan time;
        [XmlIgnore]
        public TimeSpan Time
        {
            get { return time; }
            set { time = value;
                Console.WriteLine("Теперь время {1}={0}", Time, Name); 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time))); }
        }
        [XmlElement("Time")]
        public string TimeString
        {
            get
            {
                return Time.ToString();
            }
            set
            {
                Time = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : TimeSpan.Parse(value);
            }
        }
        public Task()
        {
        }

        public Task(string name, TimeSpan time)
        {
            Name = name;
            Time = time;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        

    }
    
}
