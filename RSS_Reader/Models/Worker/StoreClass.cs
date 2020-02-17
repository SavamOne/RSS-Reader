using RSS_Reader.RSS_Classes;
using System.Collections.Generic;

namespace RSS_Reader.Worker
{
    /// <summary>
    /// Класс, который хранит все необходимые данные из RSS (все что хранит класс Channel (т.е. тег channel в RSS) 
    /// + дополнительно ItemsAll - список item'ов за все время работы worker'a
    /// + дополнительно ItemsDelta - список новых(т.е. тех, которых нет в ItemsAll) item'ов, полученных с момента последнего обновления
    /// </summary>
    public class StoreClass : Channel
    {
        public IList<Item> ItemsAll { get; set; }

        public IList<Item> ItemsDelta { get; set; }
    }
}
