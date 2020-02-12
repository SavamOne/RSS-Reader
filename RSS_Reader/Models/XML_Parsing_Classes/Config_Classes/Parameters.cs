using RSS_Reader.XML_Parser;

namespace RSS_Reader.Config_Classes
{
    public class Parameters : IXML
    {
        [XMLProperty("source")]
        public string URL { get; set; }

        [XMLProperty("updateInterval")]
        public double Interval { get; set; }

        public Parameters(string url, double interval)
        {
            URL = url;
            Interval = interval;
        }

        public Parameters() { }
    }
}
