using System;

namespace RSS_Reader.XML_Parser
{
    class XMLPropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public XMLPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

    }
}
