using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Converters
{
    public class ListStringTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is String))
            {
                return base.ConvertFrom(context, culture, value);
            }

            string list = value.ToString();
            List<string> ls = new List<string>();
            ls.AddRange(list.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries));
            return ls;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is List<string>))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            return value.ToString();
        }
    }
}
