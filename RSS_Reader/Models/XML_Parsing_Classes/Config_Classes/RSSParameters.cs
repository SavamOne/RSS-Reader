using RSS_Reader.XML_Parser;

namespace RSS_Reader.Config_Classes
{
    /// <summary>
    /// Структура элемента XML, необходимый для чтения/записи информации о RSS-канале
    /// +передается как аргумент в RSSWorker
    /// </summary>
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
