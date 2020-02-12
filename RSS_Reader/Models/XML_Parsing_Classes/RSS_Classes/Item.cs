using RSS_Reader.XML_Parser;
using System;
using System.Collections.Generic;

namespace RSS_Reader.RSS_Classes
{
    public class Item : IXML
    {
        [XMLProperty("title")]
        public string Title { get; set; }

        [XMLProperty("link")]
        public Uri Link { get; set; }

        [XMLProperty("description")]
        public string Description { get; set; }

        [XMLProperty("pubDate")]
        public DateTime PubDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Item item)
            {
                if (item == null)
                    return false;

                if (ReferenceEquals(this, item))
                    return true;

                if (string.Compare(Title, item.Title) != 0)
                    return false;

                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Title?.GetHashCode() ?? throw new Exception("Title is null. Cannot get hashcode");
        }
    }
}
