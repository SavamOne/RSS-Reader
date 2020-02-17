using System;

namespace RSS_Reader.XML_Parser
{
    /// <summary>
    /// Аттрибут, которым необходимо помечать сериализруемые/дессериализруемые свойства. 
    /// propertyName - значение, которое определяет тег XML-узла
    /// </summary>
    public class XMLPropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public XMLPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

    }
}
