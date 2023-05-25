using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StopwatchApp;
namespace StopwatchApp
{
    public class MySerializer
    {
        //it takes a lot of time
        public void Serializer(string path, Dictionary<DateTime, Dictionary<string, TimeSpan>> dict)
        {
            XElement a = new XElement("Root");
            foreach (KeyValuePair<DateTime, Dictionary<string, TimeSpan>> pair1 in dict)
            {
                
               string b=pair1.Key.ToString("yyyy-MM-dd");
                string c = ("date_05-2023-25");
               Console.WriteLine(c);
                
                    XElement child = new XElement(c, "");
                    Console.WriteLine(child);
                foreach (KeyValuePair<string, TimeSpan> pair2 in pair1.Value)
                {
                    XElement childValue = new XElement(pair2.Key, pair2.Value.ToString());
                    child.Add(childValue);
                }
                a.Add(child);
            }
            FileStream stream = new FileStream(path, FileMode.Create);
            a.Save(stream);
            stream.Close();
            Console.WriteLine("Serialized XElement: {0}" ,a);
        }
        public Dictionary<DateTime, Dictionary<string, TimeSpan>> Deserializer(string path)
        {
            Console.WriteLine("OpenGate");
            FileStream stream = new FileStream(path, FileMode.Open);
            Console.WriteLine("Loadind");
            XElement rootElement = XElement.Load(stream);
            Console.WriteLine("rootElement:\n {0}", rootElement);
            Dictionary<DateTime, Dictionary<string, TimeSpan>> dict = new Dictionary<DateTime, Dictionary<string, TimeSpan>>();
            Console.WriteLine("rootElement in cycle:");
            foreach (XElement g in rootElement.Elements())
            {
                foreach (XElement k in g.Elements())
                {
                    DateTime parsed = DateTime.Parse(g.Name.LocalName.ToString().Substring(5)); 
                    if (!dict.ContainsKey(parsed))
                        dict.Add(parsed, new Dictionary<string, TimeSpan>());
                    dict[parsed][k.Name.LocalName] =TimeSpan.Parse( k.Value);
                    Console.WriteLine("{0}  {1}  {2}", g.Name.LocalName, k.Name.LocalName, k.Value);

                }
            }
            Console.WriteLine("Dict:");
            foreach (var pair1 in dict)
            {
                foreach (var pair2 in pair1.Value)
                {
                    Console.WriteLine("{0}  {1}  {2}", pair1.Key.ToString(), pair2.Key.ToString(), pair2.Value.ToString());
                }

            }
            return dict;
        }
    }
}
