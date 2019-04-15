using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

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
            return !string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Capitalize the first letter of the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
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
            if (string.IsNullOrEmpty(str))
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

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static void Remove<T>(this ObservableCollection<T> collection, Func<T,bool> predicate)
        {
            foreach (var item in collection.ToList())
            {
                if (predicate(item))
                {
                    collection.Remove(item);
                }
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

    public static class IListExtensionMethods
    {
        public static int FindItemIndex<T>(this IList<T> list, Func<T,bool> condition)
        {
            for(int x = 0; x<=list.Count;x++)
            {
                if (condition(list[x]))
                {
                    return x;
                }
            }
            return -1;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> list)
        {
            return new ObservableCollection<T>(list);

        }
    }


}
