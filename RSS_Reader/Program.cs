using RSS_Reader.RSS_Classes;
using RSS_Reader.XML_Parser;
using System.Net;
using System.Xml;

namespace RSS_Reader
{
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
            var res = XMLParser.Parse<Channel>(xRoot["channel"]);

        }
    }
}
