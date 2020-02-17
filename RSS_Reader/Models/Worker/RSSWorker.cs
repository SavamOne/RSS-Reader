using RSS_Reader.Config_Classes;
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
    /// <summary>
    /// Главный Worker, который раз в Interval отправляет запрос на RSS-ссылку(Source) и собирает с него информацию
    /// </summary>
    public class RSSWorker : StoreClass, IWorker
    {
        public delegate void newItemsAdded(StoreClass store);

        public event newItemsAdded OnNewItemsAdded;

        public RSSParameters Param { get; }

        private DispatcherTimer Timer { get; }

        public double Interval { 
            get => Param.Interval; 
            set
            {
                Param.Interval = value;

                if(Timer!= null)
                {
                    Timer.Stop();
                    Timer.Interval = TimeSpan.FromSeconds(Param.Interval);
                    Timer.Start();
                }
            }
        }

        public string Source { get => Param.URL; }

        private XmlDocument Document { get; }

        private object Locker { get; } = new object();

        
        public RSSWorker(RSSParameters param)
        {
            //Для работы с https
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            Param = param;

            Document = new XmlDocument();

            ItemsAll = new List<Item>();
            ItemsDelta = new List<Item>();

            Timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(Interval)
            };

            Timer.Tick += (async (s, e) => await DoWorkAsync());
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


        /// <summary>
        /// Метод десериализует в объект типа Channel (в данном случае в сам RSSWorker, т.к. он наследуется от StoreClass, а он наследуется от Channel)
        /// После того, как свойства типа Channel заполнены, заполняются свойства типа StoreClass (ItemsAll и ItemsDelta). Если хотя бы 1 элемент попал в ItemsDelta,
        /// то вызывается событыие OnNewItemsAdded.
        /// </summary>
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
