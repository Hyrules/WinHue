using System;
using System.Drawing;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Color conversion class.
    /// </summary>
    internal static partial class ColorConversion
    {
        static PointF Red = new PointF(0.675f, 0.322f);
        static PointF Green = new PointF(0.4091f, 0.518f);
        static PointF Blue = new PointF(0.167f, 0.04f);
        private static int maxX = 452;
        private static int maxY = 302;

        public static double RGBtoHue(byte r,byte g, byte b)
        {
            return Math.Atan2(Math.Sqrt(3) * (g - b), 2* r - g - b);
        }

        public static XY ConvertRGBToXY(System.Drawing.Color RGB)
        {
            PointF xy = new PointF(0f, 0f);

            float red = RGB.R / 255f;
            float green = RGB.G / 255f;
            float blue = RGB.B / 255f;

            float r = (red > 0.04045f) ? (float)Math.Pow((red + 0.055f) / (1.0f + 0.055f), 2.4f) : (red / 12.92f);
            float g = (green > 0.04045f) ? (float)Math.Pow((green + 0.055f) / (1.0f + 0.055f), 2.4f) : (green / 12.92f);
            float b = (blue > 0.04045f) ? (float)Math.Pow((blue + 0.055f) / (1.0f + 0.055f), 2.4f) : (blue / 12.92f);

            float X = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            float Y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            float Z = r * 0.0193f + g * 0.1192f + b * 0.9505f;

            float cx = X / (X + Y + Z);
            float cy = Y / (X + Y + Z);

            if (cx == float.NaN)
            {
                cx = 0.0f;
            }

            if (cy == float.NaN)
            {
                cy = 0.0f;
            }
            PointF xyPoint = new PointF(cx, cy);

            if (!CheckIfInLampGamut(xyPoint))
            {
                PointF pAB = getClosestPointToPoints(Red, Green, xyPoint);
                PointF pAC = getClosestPointToPoints(Blue, Red, xyPoint);
                PointF pBC = getClosestPointToPoints(Green, Blue, xyPoint);

                float dAB = getDistanceBetweenTwoPoints(xyPoint, pAB);
                float dAC = getDistanceBetweenTwoPoints(xyPoint, pAC);
                float dBC = getDistanceBetweenTwoPoints(xyPoint, pBC);

                float lowest = dAB;
                PointF closestPoint = pAB;

                if (dAC < lowest)
                {
                    lowest = dAC;
                    closestPoint = pAC;
                }
                if (dBC < lowest)
                {
                    lowest = dBC;
                    closestPoint = pBC;
                }

                xy.X = closestPoint.X;
                xy.Y = closestPoint.Y;
            }

            XY XYColor = new XY() { x = Convert.ToDecimal(cx), y = Convert.ToDecimal(cy) };
       /*     XYColor.X = cx;
            XYColor.Y = cy;
            XYColor.Brightness = Y;*/

            return XYColor;
        }

        public static float getDistanceBetweenTwoPoints(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            return dist;
        }

        public static bool CheckIfInLampGamut(PointF XYCoordinates)
        {
            bool IsReachable = false;

            PointF v1 = new PointF(Green.X - Red.X, Green.Y - Red.Y);
            PointF v2 = new PointF(Blue.X - Red.X, Blue.Y - Red.Y);
            PointF q = new PointF(XYCoordinates.X - Red.X, XYCoordinates.Y - Red.Y);

            float s = CrossProduct(q, v2) / CrossProduct(v1, v2);
            float t = CrossProduct(v1, q) / CrossProduct(v1, v2);

            if ((s >= 0.0f) && (t >= 0.0f) && (s + t <= 1.0f))
            {
                IsReachable = true;
            }
            else
            {
                IsReachable = false;
            }

            return IsReachable;
        }

        private static PointF getClosestPointToPoints(PointF A, PointF B, PointF P)
        {
            PointF AP = new PointF(P.X - A.X, P.Y - A.Y);
            PointF AB = new PointF(B.X - A.X, B.Y - A.Y);

            float ab2 = AB.X * AB.X + AB.Y * AB.Y;
            float ap_ab = AP.X * AB.X + AP.Y * AB.Y;
            float t = ap_ab / ab2;
            if (t < 0.0f)
            {
                t = 0.0f;
            }
            else
            {
                if (t > 1.0f)
                    t = 1.0f;
            }

            PointF newPoint = new PointF(A.X + AB.X * t, A.Y + AB.Y * t);
            return newPoint;
        }

        private static float CrossProduct(PointF p1, PointF p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        /// <summary>
        /// Convert XY to RGB
        /// </summary>
        /// <param name="ColorCoordinates"></param>
        /// <param name="brightness"></param>
        /// This Conversion Method has been borrowed form Q42.HueAPI and was not created by me. https://github.com/Q42/Q42.HueApi I only altered it to output a System.Windows.Media.Color
        /// <returns></returns>
        public static System.Windows.Media.Color ConvertXYToColor(PointF ColorCoordinates, float brightness)
        {
            PointF xy = ColorCoordinates;

            float factor = 10000f;

            int closestValue = Int32.MaxValue;
            int closestX = 0, closestY = 0;
            double fX = ColorCoordinates.X;
            double fY = ColorCoordinates.Y;
            int intX = (int)(fX * factor);
            int intY = (int)(fY * factor);
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    int differenceForPixel = 0;
                    differenceForPixel += Math.Abs(xArray[x, y] - intX);
                    differenceForPixel += Math.Abs(yArray[x, y] - intY);
                    if (differenceForPixel < closestValue)
                    {
                        closestX = x;
                        closestY = y;
                        closestValue = differenceForPixel;
                    }
                }
            }
            int color = cArray[closestX, closestY];
            int red = (color >> 16) & 0xFF;
            int green = (color >> 8) & 0xFF;
            int blue = color & 0xFF;

            return System.Windows.Media.Color.FromRgb((byte)red,(byte) green,(byte) blue); ;
        }


        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private static String RGBConverter(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }


        public static void HSLToRgb(double h, double l, double s,out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }
    }

    public class HueRGBColor
    {
        public float R;
        public float G;
        public float B;
        public float A = 1.0f;

    }

    public class HueXYColor
    {
        public float X;
        public float Y;
        public float Brightness;
    }



    
}
