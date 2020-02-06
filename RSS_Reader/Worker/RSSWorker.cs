using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using RSS_Reader.RSS_Classes;
using System.Xml;
using RSS_Reader.XML_Parser;

namespace RSS_Reader.Worker
{
    class RSSWorker : Channel, IWorker
    {
        public Timer Timer { get; set; }

        public double Interval { get; set; }

        public string Source { get; set; }

        public bool WorkIsDone { get; set; }

        public XmlDocument Document { get; set; }

        object locker = new object();

        public List<Item> ItemsAll { get; set; }


        public RSSWorker(string source, ulong timeout)
        {
            Interval = timeout;
            Source = source;

            Document = new XmlDocument();

            WorkIsDone = true;

            ItemsAll = new List<Item>();

            Timer = new Timer(Interval);

            Timer.Elapsed += DoWork;
            Timer.Enabled = true;
            Timer.AutoReset = true;

            Task.Run(() => DoWork(null, null));
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            lock(locker)
            {
                Document.Load(Source);

                XMLParser.ParseInto<Channel>(Document.DocumentElement["channel"], this);

                for(int i = 0; i < Items.Count; i++)
                {
                    if(ItemsAll.Count <= i || !ItemsAll[i].Equals(Items[i]))
                    {
                        ItemsAll.Insert(i, Items[i]);
                    }
                    else
                    {
                        break;
                    }
                }


                foreach(var item in ItemsAll)
                {
                    Console.WriteLine(item.Title);
                }
                Console.WriteLine(ItemsAll.Count);
            }
                Console.WriteLine("------------------------------------------------------------------");
        }

    }
}
