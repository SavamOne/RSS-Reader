using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSS_Reader.Models
{
    /// <summary>
    /// Проверка на RSS. 
    /// Если в первых 200 байтах содержится <rss ... version="2.0">, то это - RSS
    /// </summary>
    public static class RSSChecker
    {
        static Regex RSSRegex { get; } = new Regex(@"<rss[\s\S]*version=""2.0""");
        public static bool Check(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                    char[] kek = new char[200];
                    using (StreamReader dataStream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        dataStream.Read(kek, 0, 200);
                    }


                    return RSSRegex.IsMatch(new String(kek));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return false;
        }
    }
}
