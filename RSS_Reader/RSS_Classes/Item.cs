using JetBrains.Annotations;
using RSS_Reader.XML_Parser;
using System;

namespace RSS_Reader.RSS_Classes
{
    class Item : IXML
    {
        [XMLProperty("title")]
        public string Title { get; set; }

        [XMLProperty("link")]
        public Uri Link { get; set; }

        [XMLProperty("description")]
        public string Description { get; set; }

        [CanBeNull]
        [XMLProperty("pubDate")]
        public DateTime PubDate { get; set; }
    }
}
