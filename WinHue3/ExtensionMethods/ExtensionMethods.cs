using System;
using System.Collections.Generic;
using System.Text;
using HueLib2;
using System.Collections.ObjectModel;
using System.Windows;

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
 
    }

    public static class ObjectExtensionMethods
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            if (obj == null) return false;
            return obj.GetType().GetProperty(propertyName) != null;
        }
    }

    public static class HueObjectExtensionMethods
    {
        public static string GetName(this HueObject obj)
        {
            return obj.HasProperty("name") ? obj.GetType().GetProperty("name").GetValue(obj).ToString() : "Unknown";
        }

        public static void SetName(this HueObject obj, string newName)
        {
            if (newName == null) return;
            if (!obj.HasProperty("name")) return;
            obj.GetType().GetProperty("name").SetValue(obj, newName);
        }
    }

    public static class BridgeExtensionMethods
    {
        public static void ShowErrorMessages(this Bridge bridge)
        {
            StringBuilder errors = new StringBuilder();
            errors.AppendLine(GlobalStrings.Error_WhileCreatingObject);
            foreach (Message error in bridge.lastMessages)
            {
                errors.AppendLine("");
                errors.AppendLine(error.ToString());
            }

            MessageBox.Show(errors.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static class ObservableCollectionExtensionMethods
    {
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
