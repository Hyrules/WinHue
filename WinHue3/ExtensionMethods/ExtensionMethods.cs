using System;
using System.Collections.Generic;
using System.Text;
using HueLib2;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;

namespace WinHue3
{

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
    }

    public static class BridgeExtensionMethods
    {
        public static void ShowErrorMessages(this Bridge bridge)
        {
            StringBuilder errors = new StringBuilder();
            errors.AppendLine(GlobalStrings.Error_WhileCreatingObject);
            foreach (Error error in bridge.lastMessages.ErrorMessages)
            {
                errors.AppendLine("");
                errors.AppendLine(error.ToString());
            }

            MessageBox.Show(errors.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                throw new ArgumentNullException("collection");
            }
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
