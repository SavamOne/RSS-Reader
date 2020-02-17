using RSS_Reader.Config_Classes;
using RSS_Reader.XML_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;

namespace RSS_Reader.Utils
{
    /// <summary>
    /// Чтение и запись конфига программы
    /// </summary>
    public static class ConfigReaderWriter
    {
        static string FileName { get; }

        static string FileError { get; }

        static Timer Timer { get; }

        static IList<RSSParameters> Data;

        static ConfigReaderWriter()
        {
            FileName = "data.xml";
            FileError = "data_error.xml";

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

        /// <summary>
        /// Читает XML-файл именем FileName и возвращается список из объектов типа RSSParameter
        /// </summary>
        public static IList<RSSParameters> Read()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(FileName);
                    IList<RSSParameters> list = XMLParser.DeserializeList<List<RSSParameters>>(document.DocumentElement, "parameter");
                    if (list != null)
                        return list;
                }
            }
            catch(Exception e)
            {
                /// Если произошло исключение, например, XML имеет неправильный формат, то переименовать этот файл в "файл с ошшибкой"
                try
                {
                    File.Move(FileName, FileError);
                }
                catch {}
                Console.WriteLine(e.ToString());
           }

            ///Если прочитать не удалось, то по умолчанию из источников будет только Habr.com (как написано в ТЗ)
            return new List<RSSParameters>() { new RSSParameters("https://habr.com/ru/rss/interesting/", 30)}; 
        }

        /// <summary>
        /// Записывает список из RSSParameter в XML-файл с именем FileName
        /// Запись происходит не сразу, а через 1 секунду, чтобы подождать еще изменения в списке из RSSParameter
        /// </summary>
        public static void Write(IList<RSSParameters> source)
        {
            Timer.Stop();

            Data = source;

            Timer.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                XmlDocument document = XMLParser.SerializeList(Data, "items", "parameter");
                using (XmlWriter reader = XmlWriter.Create(FileName, WriterSettings))
                {
                    document.Save(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
