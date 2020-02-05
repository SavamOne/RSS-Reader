using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSS_Reader
{
    class XMLPropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public XMLPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

    }



    class Channel
    {
        [XMLProperty("title")]
        public string Title { get; set; }
        [XMLProperty("link")]
        public Uri Link { get; set; }
        [XMLProperty("description")]
        public string Description { get; set; }

        [XMLProperty("pubDate")]
        public DateTime PubDate { get; set; }

        [XMLProperty("image")]
        public Image Image { get; set; }

        [XMLProperty("item")]
        public IEnumerable<Item> Items { get; set; }
    }

    class Item
    {
        [XMLProperty("title")]
        public string Title { get; set; }
        [XMLProperty("link")]
        public Uri Link { get; set; }
        [XMLProperty("description")]
        public string Description { get; set; }

        [XMLProperty("pubDate")]
        public DateTime PubDate { get; set; }
    }


    class Image
    {
        [XMLProperty("link")]
        public Uri Link { get; set; }

        [XMLProperty("url")]
        public Uri Url { get; set; }
    }

    class Test
    {
        public T ParseXML<T>(XmlNodeList xmlNodeList) where T : new()
        {
            var item = new T();

            var properties = item.GetType().GetProperties()
                                           .Where
                                           (
                                                x => Attribute.IsDefined(x, typeof(XMLPropertyAttribute))
                                           )
                                           .ToDictionary
                                           (
                                                x => ((XMLPropertyAttribute)x.GetCustomAttribute(typeof(XMLPropertyAttribute))).PropertyName
                                           );

            int i = 0;
            foreach (XmlNode node in xmlNodeList)
            {

                if (properties.TryGetValue(node.Name, out var value))
                {
                    try
                    {
                        var x = Convert.ChangeType(node.InnerText, value.PropertyType);
                        value.SetValue(item, x);
                    }
                    catch
                    {
                        if (value.PropertyType == typeof(Uri))
                        {
                            value.SetValue(item, new Uri(node.InnerText));
                        }
                        else if(value.PropertyType.IsGenericType)
                        {
                            var type = value.PropertyType.GetGenericArguments()[0];
                            MethodInfo method = typeof(Test).GetMethod("ParseXML");
                            MethodInfo generic = method.MakeGenericMethod(type);

                            var listType = typeof(List<>);
                            var constructedListType = listType.MakeGenericType(type);

                            var lst = Activator.CreateInstance(constructedListType);


                            for(int j = i; j < xmlNodeList.Count; j++)
                            {
                                dynamic readEvent = generic.Invoke(this, new object[] { xmlNodeList[j].ChildNodes });
                                lst.GetType().GetMethod("Add").Invoke(lst, new[] { readEvent });
                            }
                            
                            value.SetValue(item, lst);
                            return item;
                        }
                        else if (!value.PropertyType.IsPrimitive)
                        {
                            MethodInfo method = typeof(Test).GetMethod("ParseXML");
                            MethodInfo generic = method.MakeGenericMethod(value.PropertyType);
                            dynamic readEvent = generic.Invoke(this, new object[] { node.ChildNodes });
                            value.SetValue(item, readEvent);
                        }
                    }
                }
                i++;
            }
            return item;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;


            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("https://lenta.ru/rss/news");
            XmlElement xRoot = xDoc.DocumentElement;

            List<Channel> lst = new List<Channel>();

            Test test = new Test();

            var res = test.ParseXML<Channel>(xRoot["channel"].ChildNodes);

            Console.Read();
        }
    }
}
