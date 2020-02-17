using RSS_Reader.RSS_Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace RSS_Reader.XML_Parser
{
    /// <summary>
    /// Собственная реализация сереализатора/десереализатора XML.
    /// Поддерживает типы, которые выполняют IXML, System.Uri, типы, выполняющие IConvertible, а также списки с этими типами в качестве обобщенных.
    /// </summary>
    public static partial class XMLParser
    {
        /// <summary>
        /// Возвращает словарь свойств типа, которые содержат аттрибут XMLPropertyAttribute.
        /// Ключ - значение аттрибута, значение - PropertyInfo свойства
        /// </summary>
        /// <param name="item">Объект типа Т, для которого необходимо вернуть словарь свойств</param>
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

        /// <summary>
        /// Выполняет десереализацию типа Т. 
        /// Создает объект типа T и вызывает функцию DeserializeInto
        /// </summary>
        /// <param name="element">XML-узел, от которого необходимо начать десереализацию</param>
        public static T Deserialize<T>(XmlNode element) where T : new()
        {
            T item = new T();

            DeserializeInto(element, item);

            return item;
        }


        /// <summary>
        /// Выполняет десереализацию типа Т. 
        /// Получает список свойств объекта, затем проходит по всем детям XmlNode, если нашлось совпадение "тег ребенка" - "значение аттрибута", 
        /// выполняется заполнение этого свойства данными (либо InnerText ребенка, либо вызов FillList, FillIXML).
        /// </summary>
        /// <param name="element">XML-узел от которого необходимо начать десереализацию</param>
        /// <param name="item">Объект типа Т, в который необходимо записать информацию</param>
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

        /// <summary>
        /// Выполняет десереализацию списка. 
        /// Отличие от Deserialize в том, что DeserializeList десериализирует сразу относительно обобщенного типа этого списка, 
        /// а не относительно самого списка.
        /// </summary>
        /// <example>Если бы необходимо было десериализовать список при помощи Deserialize, нужно было создать класс-"заглушку", выполняющий IXML,
        /// со свойством - список, а также с аттрибутом для этого списка</example>
        /// <param name="element">XML-элемент от которого необходимо начать десереализацию</param>
        /// <param name="objAttrName">Тег ребенка element, который будет являться "объединяющим" для всех элементов списка</param>
        public static T DeserializeList<T>(XmlNode element, string objAttrName) where T : IList
        {
            int i = 0;
            Type type = typeof(T);
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && element != null)
                return (T)FillList(ref i, element.ChildNodes, objAttrName, type);
            return default;
        }


        /// <summary>
        /// Заполняет данными объект, выполняющий IXML. 
        /// Вызывается при помощи рефлексии метод Deserialize
        /// </summary>
        /// <param name="child">XML-элемент от которого необходимо начать десереализацию</param>
        /// <param name="type">Type типа, выполняющего IXML</param>
        private static object FillIXML(XmlNode child, Type type)
        {
            MethodInfo methodInfo = typeof(XMLParser).GetMethod("Deserialize");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(type);
            
            return genericMethod.Invoke(null, new object[] { child }); ;
        }

        /// <summary>
        /// Заполняет данными список.
        /// Пока список XML узлов одинакового имени - значит, десериализовать этот узел и добавить объект в список.
        /// Создается и заполняется список при помощи рефелксии методом Deserialize
        /// </summary>
        /// <param name="child">XML-элемент от которого необходимо начать десереализацию</param>
        /// <param name="type">Type типа, выполняющего IXML</param>
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
