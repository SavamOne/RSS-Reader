using RSS_Reader.RSS_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Reader.Worker
{
    class StoreClass : Channel
    {
        public IList<Item> ItemsAll { get; set; }

        public IList<Item> ItemsDelta { get; set; }
    }
}
