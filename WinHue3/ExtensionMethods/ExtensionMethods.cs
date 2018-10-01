using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinHue3.Annotations;
using WinHue3.Philips_Hue.HueObjects.Common;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.ExtensionMethods
{
    public static class ArrayExtensionMethod
    {
        public static string ArrayToString(this Array arr)
        {
            StringBuilder sb = new StringBuilder();
            int length = arr.Length;
            int i = 0;
            foreach (var v in arr)
            {
                sb.Append($"{v}");
                if (i != length - 1)
                {
                    sb.Append(",");
                }
                i++;
            }
            return sb.ToString();
        }

    }


    public static class StringExtensionMethods
    {
        /// <summary>
        /// Check if string is null,whitespace or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValid(this string str)
        {
            if(!string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Capitalize the first letter of the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(this string str)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("ARGH!");
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// Lowercase the first letter of the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LowerFirstLetter(this string str)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("ARGH!");
            return str.First().ToString().ToLower() + str.Substring(1);
        }

    }

    public static class ObjectExtensionMethods
    {
        /// <summary>
        /// Check if the object has a specified property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">Name of the property to check for.</param>
        /// <returns></returns>
        public static bool HasProperty(this object obj, string propertyName)
        {
            if (obj == null) return false;
            return obj.GetType().GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Check if the object is nullable
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsNullable(this object n)
        {
            return Nullable.GetUnderlyingType(n.GetType()) != null;
        }

        public static string GetHueType(this object obj)
        {
            HueType ht = obj.GetType().GetCustomAttribute<HueType>();
            return ht?.HueObjectType;
        }

      /*  public static List<PropertyInfo> GetListHueProperties(this  obj)
        {
            if (obj is Type) return ((Type) obj).GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToList();
            return obj?.GetType().GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToList();
        }*/

        public static PropertyInfo[] GetArrayHueProperties(this object obj)
        {
            if (obj is Type) return ((Type)obj).GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToArray();
            return obj?.GetType().GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToArray();
        }

    }

    public static class TypeExtensionMethods
    {
        public static string GetHueType(this Type type)
        {
            HueType ht = type.GetCustomAttribute<HueType>();
            return ht?.HueObjectType;
        }

        public static PropertyInfo[] GetHueProperties(this Type type)
        {
            return type.GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToArray();
        }

        public static List<PropertyInfo> GetListHueProperties(this Type type)
        {
            return type.GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToList();
        }

        public static bool HasHueProperties(this Type type)
        {
            return type.GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(HuePropertyAttribute))).ToList().Count > 0;
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                                      | BindingFlags.Public | BindingFlags.Instance);
        }
    }

    public static class ObservableCollectionExtensionMethods
    {
        /// <summary>
        /// Find the index of the object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var item in collection)
            {
                if (predicate(item))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Add a range of item to the observablecollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oc"></param>
        /// <param name="collection"></param>
        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            if(oc == null)
                oc = new ObservableCollection<T>();

            foreach (var item in collection)
            {
                oc.Add(item);
            }

        }

        
    }

    public static class ArrayExtensionMethods
    {
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }

    public static class DictionaryExtensionMethods
    {
        public static List<T> ToHueList<T>(this Dictionary<string, T> dic) where T : IHueObject
        {
            List<T> newlist = new List<T>();

            foreach (KeyValuePair<string, T> kvp in dic)
            {
                T obj = kvp.Value;
                obj.Id = kvp.Key;
                newlist.Add(obj);
            }

            return newlist;
        }

    }

    public static class BitmapImageExtensionMethods
    {
        public static string ToBase64(this BitmapImage img)
        {
            byte[] data;
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                png.Save(ms);
                data = ms.ToArray();
            }

            return Convert.ToBase64String(data);
        }

        
    }

    /*
    public static class CommonPropertiesExtensionMethods
    {

        public static void CopyValues(this CommonProperties commonprop, CommonProperties target, CommonProperties source)
        {
            Type t = typeof(CommonProperties);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

    }*/
}
