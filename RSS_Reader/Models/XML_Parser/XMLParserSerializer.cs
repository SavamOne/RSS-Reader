using RSS_Reader.RSS_Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace RSS_Reader.XML_Parser
{
   public static partial class XMLParser
   {
        public static XmlDocument Serialize(object item, string rootName)
        {
            XmlDocument document = new XmlDocument();
            document.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement elem = document.CreateElement(rootName);
            SerializeInto(item, elem);

            document.AppendChild(elem);

            return document;
        }

        public static XmlDocument SerializeList<T>(T list, string rootName, string listAttrName) where T : IEnumerable
        {
            XmlDocument document = new XmlDocument();
            document.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement root = document.CreateElement(rootName);

            if (list != null)
                foreach (var i in list)
                {
                    var elem = root.OwnerDocument.CreateElement(listAttrName);
                    SerializeInto(i, elem);
                    root.AppendChild(elem);
                }

            document.AppendChild(root);

            return document;
        }

        public static void SerializeInto(object item, XmlElement xml)
        {
            var propertiesDict = GetPropertyInfo(item);

            foreach (var pair in propertiesDict)
            {
                PropertyInfo property = pair.Value;
                string attrName = pair.Key;

                try
                {
                    if (property.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        var objEnum = property.GetValue(item) as IEnumerable;
                        if (objEnum != null)
                            foreach (var i in objEnum)
                            {
                                var elem = xml.OwnerDocument.CreateElement(attrName);
                                SerializeInto(i, elem);
                                xml.AppendChild(elem);
                            }
                    }

                    else if (typeof(IXML).IsAssignableFrom(property.PropertyType))
                    {
                        var elem = xml.OwnerDocument.CreateElement(attrName);
                        SerializeInto(property.GetValue(item), elem);
                        xml.AppendChild(elem);
                    }

                    else
                    {
                        var elem = xml.OwnerDocument.CreateElement(attrName);
                        elem.InnerText = property.GetValue(item).ToString();
                        xml.AppendChild(elem);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
