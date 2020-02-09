using RSS_Reader.RSS_Classes;
using RSS_Reader.Worker;
using System.Collections.ObjectModel;

namespace RSS_Reader
{
    class WPF_RSSWorker : RSSWorker
    {
        new public ObservableCollection<Item> Items { get;}
        public WPF_RSSWorker(string source, double interval) : base(source, interval) 
        {
            Items = new ObservableCollection<Item>();
            OnNewItemsAdded += MainWidnowViewModel_OnNewItemsAdded;
            Start();
        }

        private void MainWidnowViewModel_OnNewItemsAdded(StoreClass store)
        {
            for (int i = 0; i < store.ItemsDelta.Count; i++)
            {
                Items.Insert(i, store.ItemsDelta[i]);
            }
        }
    }
}
