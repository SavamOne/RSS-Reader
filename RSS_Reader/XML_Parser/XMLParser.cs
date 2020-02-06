using RSS_Reader.RSS_Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace RSS_Reader.XML_Parser
{
   static class XMLParser
   {
        public static Dictionary<string, PropertyInfo> GetPropertyInfo<T>(T item)
        {
            return item.GetType().GetProperties()
                    .Where
                    (
                        x => Attribute.IsDefined(x, typeof(XMLPropertyAttribute))
                    )
                    .ToDictionary
                    (
                        x => ((XMLPropertyAttribute)x.GetCustomAttribute(typeof(XMLPropertyAttribute))).PropertyName
                    );
        }


        public static T Parse<T>(XmlNode element) where T : new()
        {
            var elementChilds = element.ChildNodes;
            var item = new T();
            var propertiesDict = GetPropertyInfo(item);

            for (int i = 0; i < elementChilds.Count; i++)
            {
                var child = elementChilds[i];
                if (propertiesDict.TryGetValue(child.Name, out var property))
                {
                    object toSet = null;

                    if (typeof(IConvertible).IsAssignableFrom(property.PropertyType))
                        toSet = Convert.ChangeType(child.InnerText, property.PropertyType);

                    else if (property.PropertyType == typeof(Uri))
                        toSet = new Uri(child.InnerText);

                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        toSet = FillNumerable(ref i, elementChilds, child, property);

                    else if (typeof(IXML).IsAssignableFrom(property.PropertyType))
                        toSet = Fill(child, property);

                    property.SetValue(item, toSet);
                }
            }
            return item;
        }

        private static object Fill(XmlNode child, PropertyInfo property)
        {
            MethodInfo methodInfo = typeof(XMLParser).GetMethod("Parse");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(property.PropertyType);
            
            return genericMethod.Invoke(null, new object[] { child }); ;
        }

        private static object FillNumerable(ref int index, XmlNodeList elementChilds, XmlNode lastSeenChild, PropertyInfo property)
        {
            Type genericType = property.PropertyType.GetGenericArguments()[0];
            MethodInfo methodInfo = typeof(XMLParser).GetMethod("Parse");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(genericType);

            Type lstType = typeof(List<>).MakeGenericType(genericType);
            object lst = Activator.CreateInstance(lstType);

            for (; index < elementChilds.Count && elementChilds[index].Name == lastSeenChild.Name; index++)
            {
                object lstElem = genericMethod.Invoke(null, new object[] { elementChilds[index] });
                lst.GetType().GetMethod("Add").Invoke(lst, new[] { lstElem });
            }

            return lst;
        }
    }
}
