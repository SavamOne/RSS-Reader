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
        private static Dictionary<string, PropertyInfo> GetPropertyInfo<T>(T item)
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

        public static T Deserialize<T>(XmlNode element) where T : new()
        {
            T item = new T();

            DeserializeInto(element, item);

            return item;
        }


        public static void DeserializeInto<T>(XmlNode element, T item) where T : new()
        {
            var elementChilds = element.ChildNodes;
            var propertiesDict = GetPropertyInfo(item);

            for (int i = 0; i < elementChilds.Count; i++)
            {
                var child = elementChilds[i];
                if (propertiesDict.TryGetValue(child.Name, out var property))
                {
                    object toSet = null;

                    if (typeof(IConvertible).IsAssignableFrom(property.PropertyType))
                    {
                        try
                        {
                            toSet = Convert.ChangeType(child.InnerText, property.PropertyType);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                        

                    else if (property.PropertyType == typeof(Uri))
                    {
                        if (Uri.TryCreate(child.InnerText, UriKind.RelativeOrAbsolute, out Uri res))
                            toSet = res;
                    }

                    else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                        toSet = FillList(ref i, elementChilds, child.Name, property.PropertyType);

                    else if (typeof(IXML).IsAssignableFrom(property.PropertyType))
                        toSet = FillIXML(child, property.PropertyType);

                    property.SetValue(item, toSet);
                }
            }
        }

        public static T DeserializeList<T>(XmlNode element, string objAttrName) where T : IList
        {
            int i = 0;
            Type type = typeof(T);
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && element != null)
                return (T)FillList(ref i, element.ChildNodes, objAttrName, type);
            return default;
        }

        private static object FillIXML(XmlNode child, Type type)
        {
            MethodInfo methodInfo = typeof(XMLParser).GetMethod("Deserialize");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(type);
            
            return genericMethod.Invoke(null, new object[] { child }); ;
        }

        private static object FillList(ref int index, XmlNodeList elementChilds, string neededAttribute, Type type)
        {
            Type genericType = type.GetGenericArguments()[0];
            MethodInfo methodInfo = typeof(XMLParser).GetMethod("Deserialize");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(genericType);

            Type lstType = typeof(List<>).MakeGenericType(genericType);
            object lst = Activator.CreateInstance(lstType);

            for (; index < elementChilds.Count && elementChilds[index].Name == neededAttribute; index++)
            {
                object lstElem = genericMethod.Invoke(null, new object[] { elementChilds[index] });
                lst.GetType().GetMethod("Add").Invoke(lst, new[] { lstElem });
            }

            return lst;
        }
    }
}
