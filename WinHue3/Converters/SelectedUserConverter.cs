using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using HueLib2;

namespace WinHue3.Converters
{
    public class SelectedUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*
            if (value == null) return null;
            KeyValuePair<string, Whitelist> kvp = (KeyValuePair<string, Whitelist>) value;

            Whitelist wl = kvp.Value;
            wl.id = kvp.Key;
            return wl;*/
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            KeyValuePair<string, Whitelist> kvp = (KeyValuePair<string, Whitelist>)value;

            Whitelist wl = kvp.Value;
            wl.id = kvp.Key;
            return wl;
        }
    }
}
