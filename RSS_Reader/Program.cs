using RSS_Reader.RSS_Classes;
using RSS_Reader.Worker;
using RSS_Reader.XML_Parser;
using System.Net;
using System.Threading;
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


            var worker = new RSSWorker("https://lenta.ru/rss/news", 30000);

            System.Console.ReadLine();
        }
    }
}
