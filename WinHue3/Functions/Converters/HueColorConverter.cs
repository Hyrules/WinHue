using System;
using System.Globalization;
using System.Windows.Data;
using WinHue3.Colors;

namespace WinHue3.Converters
{
    public class HueColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Media.Color.FromRgb(255, 0, 0);
            double val = ((double)value) / 273.06;
            System.Windows.Media.Color color = new HSLColor(val, 240, 120);
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not important we never convert back;
            throw new NotImplementedException();

        }
    }
}
