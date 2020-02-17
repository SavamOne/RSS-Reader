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
        /// <summary>
        /// Выполняет сереализацию объекта. 
        /// Создает XmlDocument с корневым узлом rootName и вызывает функцию SerializeInto
        /// </summary>
        /// <param name="item">объект, который необходимо сериализовать</param>
        /// <param name="rootName">Тег корневого узла в XML-документе</param>
        public static XmlDocument Serialize(object item, string rootName)
        {
            XmlDocument document = new XmlDocument();
            document.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement elem = document.CreateElement(rootName);
            SerializeInto(item, elem);

            document.AppendChild(elem);

            return document;
        }


        /// <summary>
        /// Выполняет сереализацию списка. 
        /// Отличие от Serialize в том, что SerializeList сериализирует сразу объекты этого списка, а сначала сам список.
        /// </summary>
        /// <param name="list">Список, который необходимо сериализовать</param>
        /// <param name="rootName">Тег корневого узла в XML-документе</param>
        /// <param name="listAttrName">Тег ребенка element, которым будут "обворачиваться" элементы этого списка/param>
        public static XmlDocument SerializeList<T>(IEnumerable<T> list, string rootName, string listAttrName)
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

        /// <summary>
        /// Выполняет десереализацию объекта. 
        /// Получает список свойств объекта, затем проходит по всем детям XmlNode, если нашлось совпадение "тег ребенка" - "значение аттрибута", 
        /// создается XML-узел с данными из свойства этого объекта (значение свойства или сериализация объекта этого свойства)
        /// </summary>
        /// <param name="item">Объект, который необходимо сериализовать/param>
        /// <param name="xml">XML-узел, в который необходимо записать данные</param>
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
