using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class SensorToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return 0;
            switch ((string)value)
            {
                case "CLIPGenericFlag":
                    return 0;
                case "CLIPGenericStatus":
                    return 1;
                case "CLIPHumidity":
                    return 2;
                case "CLIPOpenClose":
                    return 3;
                case "CLIPPresence":
                    return 4;
                case "CLIPTemperature":
                    return 5;
                case "CLIPSwitch":
                    return 6;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "CLIPGenericFlag";
            switch ((int)value)
            {
                case 0:
                    return "CLIPGenericFlag";
                case 1:
                    return "CLIPGenericStatus";
                case 2:
                    return "CLIPHumidity";
                case 3:
                    return "CLIPOpenClose";
                case 4:
                    return "CLIPPresence";
                case 5:
                    return "CLIPTemperature";
                case 6:
                    return "CLIPSwitch";
                default:
                    return "CLIPGenericFlag";
            }
        }
    }
}
