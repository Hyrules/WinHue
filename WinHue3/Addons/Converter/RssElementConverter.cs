using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Addons.Converter
{
    public class RssElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            if (value.ToString() == string.Empty) return 0;
            switch ((string)value)
            {
                case "Title":
                    return 0;
                case "Description / Summary":
                    return 1;
                case "Category":
                    return 2;
                case "Publication Date":
                    return 3;
                default:
                    return 0;

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            switch ((int)value)
            {
                case 0:
                    return "Title";
                case 1:
                    return "Description / Summary";
                case 2:
                    return "Category";
                case 3:
                    return "Publication Date";
                default:
                    return "Title";
            }

        }
    }
}
