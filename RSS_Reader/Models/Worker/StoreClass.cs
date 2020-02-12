using RSS_Reader.RSS_Classes;
using System.Collections.Generic;

namespace RSS_Reader.Worker
{
    public class StoreClass : Channel
    {
        public IList<Item> ItemsAll { get; set; }

        public IList<Item> ItemsDelta { get; set; }
    }
}
