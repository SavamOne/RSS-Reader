using RSS_Reader.RSS_Classes;
using RSS_Reader.Worker;
using RSS_Reader.XML_Parser;
using System.Net;
using System;

namespace RSS_Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new RSSWorker("https://lenta.ru/rss/news", 30000);
            worker.OnNewItemsAdded += Worker_GotNewItems;
            worker.Start();

            var worker2 = new RSSWorker("https://habr.com/ru/rss/interesting/", 30000);
            worker2.OnNewItemsAdded += Worker_GotNewItems;
            worker2.Start();

            Console.ReadLine();
        }

        private static void Worker_GotNewItems(StoreClass store)
        {
            store.ItemsAll.ForEach(x => System.Console.WriteLine($"{store.Title} : {x.Title}"));
            Console.WriteLine($"{store.Title} : {store.ItemsAll.Count}");
        }
    }
}
