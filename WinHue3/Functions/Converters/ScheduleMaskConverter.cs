using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    [ValueConversion(typeof(string),typeof(byte))]
    public class ScheduleMaskConverter: IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            if (value.ToString() == string.Empty) return byte.MinValue;
            byte mask = System.Convert.ToByte(value);

            if ((mask & 64) != 0)
            {
                result = result + "Mon";
            }

            if ((mask & 32) != 0)
            {
                result = result + "," + "Tue";
            }

            if ((mask & 16) != 0)
            {
                result = result + "," + "Wed";
            }

            if ((mask & 8) != 0)
            {
                result = result + "," + "Thu";
            }

            if ((mask & 4) != 0)
            {
                result = result + "," + "Fri";
            }

            if ((mask & 2) != 0)
            {
                result = result + "," + "Sat";
            }

            if ((mask & 1) != 0)
            {
                result = result + "," + "Sun";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte mask = 0;
            string[] val = value.ToString().Split(',');

            foreach (string s in val)
            {
                switch (s)
                {
                    case "Mon":
                        mask = System.Convert.ToByte(mask + 64);
                        break;
                    case "Tue":
                        mask = System.Convert.ToByte(mask + 32);
                        break;
                    case "Wed":
                        mask = System.Convert.ToByte(mask + 16);
                        break;
                    case "Thu":
                        mask = System.Convert.ToByte(mask + 8);
                        break;
                    case "Fri":
                        mask = System.Convert.ToByte(mask + 4);
                        break;
                    case "Sat":
                        mask = System.Convert.ToByte(mask + 2);
                        break;
                    case "Sun":
                        mask = System.Convert.ToByte(mask + 1);
                        break;
                }

            }
            return $"{mask:D3}";
        }
    }
}
