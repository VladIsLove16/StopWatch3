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
    public class serializ
    {
        public void Ser(string path)
        {
            XElement a = new XElement("Root");
            XElement Child = new XElement("DAy1");
            XElement Child2 = new XElement("Day2");
            XElement Child3 = new XElement("Day3");
            XElement ChildValue = new XElement("Name", "Time");
            XElement ChildValue2 = new XElement("Name2", "Time2");
            XElement ChildValue3 = new XElement("Name3", "Time3");
            Child.Add(ChildValue);
            Child.Add(ChildValue2);
            Child2.Add(ChildValue2);
            Child2.Add(ChildValue3);
            Child3.Add(ChildValue3);
            Child3.Add(ChildValue2);
            a.Add(Child);
            a.Add(Child2);
            FileStream stream = new FileStream(path, FileMode.Create);
            a.Save(stream);
            Console.WriteLine(a);
            Console.WriteLine("OpenGate");
            stream.Close();
            FileStream stream2 = new FileStream(path, FileMode.Open);
            Console.WriteLine("Loadind");
            XElement rootElement = XElement.Load(stream2);
            Console.WriteLine("rootElement: {0}", rootElement);
            
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
            Console.WriteLine("Dict=");
            foreach (XElement g in rootElement.Elements())
            {
                foreach (XElement k in g.Elements())
                {

                    if (!dict.ContainsKey(g.Name.LocalName))
                        dict.Add(g.Name.LocalName, new Dictionary<string, string>());
                    dict[g.Name.LocalName][k.Name.LocalName] = k.Value;
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
        }
        public Dictionary<string, Dictionary<string, string>> deser(string path)
        {
            string filePath = path;
            FileStream stream = new FileStream(filePath, FileMode.Open);
            XElement rootElement = XElement.Load(stream);
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
            
                foreach (var el in rootElement.Elements())
                {
                        foreach (var el2 in el.Elements())
                    {
                     dict[el.Name.LocalName.ToString()][el2.Name.LocalName.ToString()] = el2.Value.ToString();
                    }
                }
                foreach(var pair1 in dict)
            {
                foreach(var pair2 in pair1.Value)
                {
                    Console.WriteLine("{0}  {1}  {2}", pair1.Key.ToString(),pair2.Key.ToString(),pair2.Value.ToString());
                }
            }
            //dict = rootElement;
            return dict;
        }
    }
}
