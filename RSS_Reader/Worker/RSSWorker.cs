using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using RSS_Reader.RSS_Classes;
using System.Xml;
using RSS_Reader.XML_Parser;
using System.Net;

namespace RSS_Reader.Worker
{
    class RSSWorker : StoreClass, IWorker
    {
        public delegate void newItemsAdded(StoreClass store);

        public event newItemsAdded OnNewItemsAdded;

        private Timer Timer { get; }

        public double Interval { get; }

        public string Source { get; }

        private XmlDocument Document { get; }

        private object Locker { get; } = new object();

        
        public RSSWorker(string source, ulong timeout)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            Interval = timeout;
            Source = source;

            Document = new XmlDocument();

            ItemsAll = new List<Item>();

            Timer = new Timer(Interval);

            Timer.Elapsed += ((s, e) => DoWork());
            Timer.AutoReset = true;
        }

        public void Start()
        {
            Timer.Start();
            Task.Run(() => DoWork());
        }

        public void Stop()
        {
            Timer.Stop();
        }

        private void DoWork()
        {
            lock (Locker)
            {
                bool isSmthngNew = false;
                Document.Load(Source);

                XMLParser.ParseInto<Channel>(Document.DocumentElement["channel"], this);

                for (int i = 0; i < Items.Count; i++)
                {
                    if (ItemsAll.Count <= i || !ItemsAll[i].Equals(Items[i]))
                    {
                        ItemsAll.Insert(i, Items[i]);
                        isSmthngNew = true;
                    }
                    else
                        break;
                }

                if (isSmthngNew)
                    OnNewItemsAdded(this);
            }
        }
    }
}
