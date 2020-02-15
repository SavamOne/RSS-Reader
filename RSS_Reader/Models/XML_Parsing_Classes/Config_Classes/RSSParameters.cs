using RSS_Reader.XML_Parser;

namespace RSS_Reader.Config_Classes
{
    public class RSSParameters : IXML
    {
        [XMLProperty("source")]
        public string URL { get; set; }

        [XMLProperty("updateInterval")]
        public double Interval { get; set; }

        public RSSParameters(string url, double interval)
        {
            URL = url;
            Interval = interval;
        }

        public RSSParameters() { }
    }
}
