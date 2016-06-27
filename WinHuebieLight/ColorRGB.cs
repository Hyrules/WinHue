using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;

using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;

namespace WinHuebieLight
{
    public class ColorRGB
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public ColorRGB()
        {
            R = 255;
            G = 255;
            B = 255;
            A = 255;
        }

        public ColorRGB(Color value)
        {
            this.R = value.R;
            this.G = value.G;
            this.B = value.B;
            this.A = value.A;
        }
        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            return c;
        }
        public static explicit operator ColorRGB(Color c)
        {
            return new ColorRGB(c);
        }


        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRGB FromHSL(double H, double S, double L)
        {
            return FromHSLA(H, S, L, 1.0);
        }

        // Given H,S,L,A in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRGB FromHSLA(double H, double S, double L, double A)
        {
            double v;
            double r, g, b;
            if (A > 1.0)
                A = 1.0;

            r = L;   // default to gray
            g = L;
            b = L;
            v = (L <= 0.5) ? (L * (1.0 + S)) : (L + S - L * S);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = L + L - v;
                sv = (v - m) / v;
                H *= 6.0;
                sextant = (int)H;
                fract = H - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            ColorRGB rgb = new ColorRGB();
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(g * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            rgb.A = Convert.ToByte(A * 255.0f);
            return rgb;
        }

        // Hue in range from 0.0 to 1.0
        public float H
        {
            get
            {
                // Use System.Drawing.Color.GetHue, but divide by 360.0F 
                // because System.Drawing.Color returns hue in degrees (0 - 360)
                // rather than a number between 0 and 1.
                return ((Color)this).GetHue() / 360.0F;
            }
        }

        // Saturation in range 0.0 - 1.0
        public float S
        {
            get
            {
                return ((Color)this).GetSaturation();
            }
        }

        // Lightness in range 0.0 - 1.0
        public float L
        {
            get
            {
                return ((Color)this).GetBrightness();
            }
        }
    }

}
