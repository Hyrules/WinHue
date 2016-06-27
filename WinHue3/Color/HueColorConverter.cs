using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3
{
    public class HueColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Media.Color.FromRgb(255, 0, 0);
            double val = ((double)value) / 273.06;
            Color color = new HSLColor(val, 240, 120);
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not important we never convert back;
            throw new NotImplementedException();

        }
    }
}
