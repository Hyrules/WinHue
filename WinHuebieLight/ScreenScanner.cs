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

    class ScreenScanner
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        protected static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        protected static extern System.IntPtr SelectObject(
            [In()] System.IntPtr hdc,
            [In()] System.IntPtr h);

        /// <summary>
        /// determines the calculation image size in percent (where 1 is 100%)
        /// </summary>
        protected float quality = 0.05f;


        /// <summary>
        /// Avarage color for entire image
        /// </summary>
        protected Color avarageColor;

        /// <summary>
        /// avarage color of strongly saturated pixels
        /// </summary>
        protected Color avarageSaturatedColor;

        /// <summary>
        /// avarage percieved luminuosity of the image
        /// </summary>
        protected float avarageLuminuosity;

        protected Thread worker = null;

        private bool bRun = false;

        public void Start()
        {
            if (worker != null)
                throw new Exception("Worker already started");
            bRun = true;
            worker = new Thread(Scanner);
            worker.Start();
        }

        public void Stop()
        {
            if (worker == null)
                throw new Exception("Worker not running");

            bRun = false;
            worker.Join();
            worker = null;
        }

        /// <summary>
        /// Avarage color in the image
        /// </summary>
        public Color AvarageColor
        {
            get
            {
                lock(this)
                {
                    return avarageColor;
                }
            }
        }

        /// <summary>
        /// This returns the average of saturated colors, ignoring values of unsaturated pixels
        /// Used to find single colorful spots in the image
        /// </summary>
        public Color AvarageSaturatedColor
        {
            get
            {
                lock (this)
                {
                    return avarageSaturatedColor;
                }
            }
        }

        public float AvarageLuma
        {
            get
            {
                lock(this)
                {
                    return avarageLuminuosity;
                }
            }
        }

        /// <summary>
        /// scanner worker thread
        /// </summary>
        protected void Scanner()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);
            
            var bmpResized = new Bitmap((int)(bmpScreenshot.Width * quality), (int)(bmpScreenshot.Height * quality));

            uint w = (uint)bmpResized.Width, h = (uint)bmpResized.Height;


            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            uint framecount = 0;
            DateTime dtStart = DateTime.Now;

            int ttkey = 0; /*Time until next keyframe is captured*/
            while (bRun)
            {

                Size sScreenshot = Screen.PrimaryScreen.Bounds.Size;

                if (ttkey == 0)
                {
                    //Keyframe - take complete screenshot.
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                Screen.PrimaryScreen.Bounds.Y,
                                                0,
                                                0,
                                                sScreenshot,
                                                CopyPixelOperation.SourceCopy);
                    
                    ttkey = 15; /*15 frames till next keyframe*/
                }
                else
                {
                    
                    /* only snapshot center of screen*/
                    Size sCrop = new Size((int)(sScreenshot.Width * 0.25), (int)(sScreenshot.Height * 0.25));

                    sScreenshot.Height -= sCrop.Height;
                    sScreenshot.Width -= sCrop.Width;

                    // Take the screenshot from the upper left corner to the right bottom corner.
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X + sCrop.Width / 2,
                                                Screen.PrimaryScreen.Bounds.Y + sCrop.Height / 2,
                                                0 + sCrop.Width / 2,
                                                0 + sCrop.Height / 2,
                                                sScreenshot,
                                                CopyPixelOperation.SourceCopy);

                    ttkey--; /*decrement time till next keyframe*/
                }

                ResizeImage(bmpScreenshot, bmpResized);

                double r = 0, g = 0, b = 0,
                    strongR = 0, strongG = 0, strongB = 0;
                UInt32 cPixels=0, strongC = 0;
                double sumLuma = 0;

                /*Calculate average ARGB Values.
                    One could possibly improve performance of scanning pixels by directly access bitmap bytes
                    but as the biggest slice of scan time is lost during CopyFromScreen its disregarded for now*/
                for (uint y = 0; y < h; y++)
                {
                    uint numBlackPixelsInRow = 0;//if the entire row is black, this is propaby a black frame
                    for (uint x = 0; x < w; x++)
                    {
                        ColorRGB clr = new ColorRGB( bmpResized.GetPixel((int)x, (int)y) );

                        if (clr.L < 0.005)
                        {
                            numBlackPixelsInRow++;
                        }


                        //if (clr.L > 0.005 && clr.L < 0.95) /*Dont account for pitch black or plain white.*/
                        {
                            cPixels++;

                            r += clr.R;
                            g += clr.G;
                            b += clr.B;

                            sumLuma += clr.L;

                            if (clr.S > 0.3f)
                            {/*This pixel is neiter black or white and so somwhat strongly saturated*/
                                strongR += clr.R;
                                strongG += clr.G;
                                strongB += clr.B;
                                strongC++;
                            }
                        }
                    }

                    if( numBlackPixelsInRow==w) /*Entire row was black*/
                    {
                        cPixels -= (uint)w; /*Kinda remove the black row.*/
                    }
                }

                
                r /= cPixels;
                g /= cPixels;
                b /= cPixels;

                if (strongC > 0)
                {
                    strongR /= strongC;
                    strongG /= strongC;
                    strongB /= strongC;
                }

                lock(this)
                {
                    avarageSaturatedColor = Color.FromArgb(
                        255, 
                        Math.Max(0, Math.Min( 255,(int)strongR)),
                        Math.Max(0, Math.Min(255, (int)strongG)),
                        Math.Max(0, Math.Min(255, (int)strongB))
                        );

                    /*Current avarage screen colour becomes setpoint for lighting*/
                    avarageColor = Color.FromArgb(
                        255,
                        Math.Max(0, Math.Min(255, (int)r)),
                        Math.Max(0, Math.Min(255, (int)g)),
                        Math.Max(0, Math.Min(255, (int)b))
                        );

                    avarageLuminuosity = (float)(sumLuma / cPixels);
                }

           
                System.Threading.Thread.Sleep(10);

            }
        }

        /// <summary>
        /// Resize an image. Target dimensions are defined by destImage
        /// </summary>
        /// <param name="srcImage">Image to be resized</param>
        /// <param name="destImage">Pre- created bitmap to receive resized Image. This also defines dimensions of the resize operation</param>
        protected static void ResizeImage(Bitmap srcImage, Bitmap destImage)
        {
            var destRect = new Rectangle(0, 0, destImage.Width, destImage.Height);


            destImage.SetResolution(srcImage.HorizontalResolution, srcImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Low;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(srcImage, destRect, 0, 0, srcImage.Width, srcImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }
    }
}
