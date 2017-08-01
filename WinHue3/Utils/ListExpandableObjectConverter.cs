using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace WinHue3.Utils
{
    public class ListExpandableObjectConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is List<string>)
                return string.Join(",", value);
            return base.ConvertTo(context, culture, value, destinationType);
        }
       
    }
}
