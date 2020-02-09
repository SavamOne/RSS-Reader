using RSS_Reader.XML_Parser;
using System;
using System.Collections.Generic;

namespace RSS_Reader.RSS_Classes
{
    class Channel : IXML
    {
        [XMLProperty("title")]
        public string Title { get; set; }

        [XMLProperty("link")]
        public Uri Link { get; set; }

        [XMLProperty("description")]
        public string Description { get; set; }

        [XMLProperty("pubDate")]
        public DateTime PubDate { get; set; }

        [XMLProperty("image")]
        public Image Image { get; set; }

        [XMLProperty("item")]
        public IList<Item> Items { get; set; }
    }
}
