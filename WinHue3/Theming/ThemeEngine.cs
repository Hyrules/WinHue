using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WinHue3.Theming
{
    public static class ThemeEngine
    {
        public static Color ChangeLightness(this Color color, float coef)
        {
            return Color.FromRgb(Convert.ToByte(color.R * coef), Convert.ToByte(color.G * coef), Convert.ToByte(color.B * coef));
        }

        public static Color InterpolateColors(Color color1, Color color2, float percentage)
        {
            double a1 = color1.A / 255.0, r1 = color1.R / 255.0, g1 = color1.G / 255.0, b1 = color1.B / 255.0;
            double a2 = color2.A / 255.0, r2 = color2.R / 255.0, g2 = color2.G / 255.0, b2 = color2.B / 255.0;

            byte a3 = Convert.ToByte((a1 + (a2 - a1) * percentage) * 255);
            byte r3 = Convert.ToByte((r1 + (r2 - r1) * percentage) * 255);
            byte g3 = Convert.ToByte((g1 + (g2 - g1) * percentage) * 255);
            byte b3 = Convert.ToByte((b1 + (b2 - b1) * percentage) * 255);
            return Color.FromArgb(a3, r3, g3, b3);
        }

        public static Color TextColorOnBackground(Color backgroundColor)
        {
            if (backgroundColor.R + backgroundColor.G + backgroundColor.B > 382)
            {
                return Color.FromRgb(0, 0, 0);
            }

            return Color.FromRgb(255,255,255);
        }


    }

}
