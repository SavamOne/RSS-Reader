using RSS_Reader.RSS_Classes;
using RSS_Reader.XML_Parser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace RSS_Reader.Worker
{
    public class RSSWorker : StoreClass, IWorker
    {
        public delegate void newItemsAdded(StoreClass store);

        public event newItemsAdded OnNewItemsAdded;

        private DispatcherTimer Timer { get; }

        public double Interval { get; private set; }

        public string Source { get; }

        private XmlDocument Document { get; }

        private object Locker { get; } = new object();

        
        public RSSWorker(string source, double interval)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            Interval = interval;
            Source = source;

            Document = new XmlDocument();

            ItemsAll = new List<Item>();
            ItemsDelta = new List<Item>();

            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(interval)
            };

            Timer.Tick += (async (s, e) => await DoWorkAsync());
        }

        public void ChangeInterval(double interval)
        {
            Interval = interval;
            Timer.Stop();
            Timer.Interval = TimeSpan.FromMilliseconds(Interval);
            Timer.Start();

            Console.WriteLine("Новый интервал");
        }

        public Task Start()
        {
            Timer.Start();
            return DoWorkAsync();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        private async Task DoWorkAsync()
        {
            bool isSmthngNew = false;
            await Task.Run(() =>
            {
                lock (Locker)
                {
                    try
                    {
                        Document.Load(Source);

                        XMLParser.DeserializeInto<Channel>(Document.DocumentElement["channel"], this);

                        ItemsDelta.Clear();
                        for (int i = 0; i < Items.Count; i++)
                        {
                            if (ItemsAll.Count <= i || !ItemsAll[i].Equals(Items[i]))
                            {
                                ItemsAll.Insert(i, Items[i]);
                                ItemsDelta.Insert(i, Items[i]);
                                isSmthngNew = true;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            });
            if (isSmthngNew)
                OnNewItemsAdded(this);
        }
    }
}
