using StopwatchApp;
using StopWatchItem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace StopwatchApp
{
    class Serializer
    {
        public static void SerializeList(ObservableCollection<StopwatchItem> stopwatches, string path)
        {
            Console.WriteLine("Сериализация Списка");
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ObservableCollection<StopwatchItem>));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            { xmlserializer.Serialize(stream, stopwatches); }
            Console.WriteLine("Сериализация Списка успешно");
        }
        public static ObservableCollection<StopwatchItem> DeserializeList(string path)
        {
            ObservableCollection<StopwatchItem> collection = new ObservableCollection<StopwatchItem>();
            Console.WriteLine("Десериализация Списка");
            XmlSerializer xmlserializer = new XmlSerializer(typeof(ObservableCollection<StopwatchItem>));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            { collection = (ObservableCollection<StopwatchItem>)xmlserializer.Deserialize(stream); }
            Console.WriteLine("Десериализация Списка успешно");
            return collection;
        }
        public static void SerializeTaskTracker(TaskTracker tracker, string path)
        {
            Console.WriteLine("Сериализация Трекера");
            XmlSerializer serializer = new XmlSerializer(typeof(TaskTracker));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, tracker);
            }
            Console.WriteLine("Сериализация Трекера завершена");
        }

        public static TaskTracker DeserializeTaskTracker(string path)
        {
            Console.WriteLine("Десериализация Трекера");
            XmlSerializer serializer = new XmlSerializer(typeof(TaskTracker));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                Console.WriteLine("Десериализация Трекера завершена");
                return (TaskTracker)serializer.Deserialize(stream);
            }
        }
        private static void SerializeTimeSpan(TimeSpan timeSpan, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TimeSpan));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, timeSpan);
            }
        }

        private static TimeSpan DeserializeTimeSpan(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TimeSpan));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (TimeSpan)serializer.Deserialize(reader);
            }
        }
    }
}
