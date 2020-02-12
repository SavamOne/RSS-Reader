using RSS_Reader.Config_Classes;
using RSS_Reader.XML_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;

namespace RSS_Reader.Utils
{
    public static class ConfigReaderWriter
    {
        static string FileName { get; } = "data.xml";
        static string FileNameSecond { get; } = "data2.xml";

        static Timer Timer { get; }

        static IList<Parameters> Data;

        static ConfigReaderWriter()
        {
            FileName = "data.xml";
            FileNameSecond = "data2.xml";

            Timer = new Timer() { Interval = 1000, AutoReset = false };
            Timer.Elapsed += Timer_Elapsed;
        }

        static XmlWriterSettings WriterSettings { get; } = new XmlWriterSettings()
        {
            NewLineChars = Environment.NewLine,
            NewLineOnAttributes = true,
            NewLineHandling = NewLineHandling.Replace,
            CloseOutput = true,
            Indent = true,
        };

        public static IList<Parameters> Read()
        {      
            try
            {
                if(File.Exists(FileName))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(FileName);
                    IList<Parameters> list = XMLParser.DeserializeList<List<Parameters>>(document.DocumentElement, "parameter");
                    if (list != null)
                        return list;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());  
            }

            return new List<Parameters>() { new Parameters("https://habr.com/ru/rss/interesting/", 30000)}; 
        }

        public static void Write(IList<Parameters> source)
        {
            Console.WriteLine("Ожидание");
            Timer.Stop();

            Data = source;

            Timer.Start();

        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Дождались");
            try
            {
                XmlDocument document = XMLParser.SerializeList(Data, "items", "parameter");
                using (XmlWriter reader = XmlWriter.Create(FileNameSecond, WriterSettings))
                {
                    document.Save(reader);
                }

                if (File.Exists(FileName))
                    File.Delete(FileName);

                File.Move(FileNameSecond, FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Записано");
        }
    }
}
