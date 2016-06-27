using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using NDesk.Options;

namespace WinHuebieLight
{

    class Program
    {

       

        static OptionSet options;

       static  bool showHelp = false;
        static Program()
        {
            options = new OptionSet()
            {
                {"l|light=", "Comma separated list of lights to use", delegate(string v)
                {
                    string[] sIds = v.Split(',');
                    foreach( string s in sIds )
                    {
                        try
                        {
                            Settings.lights.Add( int.Parse(s) );
                        }
                        catch( Exception )
                        {
                            throw new Exception("Error parsing arguments: " + s +"is no valid id");
                        }
                    }
                }
                },
                {"g|group=", "Comma separated list of groups to use", delegate(string v)
                {
                    string[] sIds = v.Split(',');
                    foreach( string s in sIds )
                    {
                        try
                        {
                            Settings.groups.Add( int.Parse(s) );
                        }
                        catch( Exception )
                        {
                            throw new Exception("Error parsing arguments: " + s +"is no valid id");
                        }
                    }
                }
                },
                {"s|sat=","Saturation boost or decrease in %", delegate(int v)
                {Settings.saturation = v/100.0f;}},
                {"b|bri=","Brightness boost or decrease in %", delegate(int v)
                {Settings.luminuosity = v/100.0f;}},
                {"minLuma=","Lowest light level to be set even in darkest scenes (0..255)", delegate(byte v)
                {Settings.minLight = v;}},
                {"maxLuma=","Highest light level to be set even in darkest scenes (0..255)", delegate(byte v)
                {Settings.maxLight = v;}},
                {"q|quality=","Screen scan quality in % (default = 5)", delegate(int v)
                {Settings.scanQuality = v/100;}},
                {"tt|transitiontime=","Transitiontime for huelights to change color (default = 3)", delegate(ushort v)
                {Settings.transitiontime = v;}},
                {"md|minDelta=","Minimum Change in lighting that is required to send update to the bridge (default 0.1)", delegate(float v)
                {Settings.minDelta = v;}},
                {"method=","AVERAGE(default), HUEBOOST, LUMAONLY", delegate(string v)
                {
                    if( String.Compare( "AVERAGE", v, true )==0 ) 
                        Settings.scanMethod = ScanMethod.Avarage;
                    else if( String.Compare( "HUEBOOST", v, true )==0 )
                        Settings.scanMethod = ScanMethod.HueBoost;
                    else if( String.Compare( "LUMAONLY", v, true )==0 )
                        Settings.scanMethod = ScanMethod.LumaOnly;
                    else
                        throw new Exception( "Invalid option for method");
                } },
                {"?|help","Print usage", delegate(string v)
                { showHelp = true; }}
            };
        }
  

        static void Main(string[] args)
        {
            /*Global exception handler dumps exception to textfile*/
            Application.ThreadException += Application_ThreadException;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                options.Parse(args);
            }
            catch( Exception ex )
            {
                Console.WriteLine("Error parsing arguments: " + ex.Message);
                options.WriteOptionDescriptions(Console.Out);
                return;
            }
            if( showHelp )
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }
            if( Settings.lights.Count==0 && Settings.groups.Count==0 )
            {
                Console.WriteLine("Chosing lights is mandatory. Specify at least one light or group to use");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            ScreenScanner scanner = new ScreenScanner();
            LightSetter ls = new LightSetter();

            try
            {
                ls.Init(scanner);
            }
            catch( Exception ex )
            {
                Console.WriteLine(ex.Message);
                return;
            }


            scanner.Start();
            ls.Start();

            do
            {
                Thread.Sleep(100);
                Console.Clear();
                Console.WriteLine("Lighting process running");
                Console.WriteLine("Average color" + scanner.AvarageColor.ToString());
                Console.WriteLine("AvarageSaturatedColor" + scanner.AvarageSaturatedColor.ToString());
                Console.WriteLine("AvarageLuminuosity = " + scanner.AvarageLuma);
                Console.WriteLine("");
                Console.WriteLine("Updates sent to bridge = " + ls.LightUpdatesSent);
                Console.WriteLine("");
                Console.WriteLine("Press any key to terminate");
            }
            while (!Console.KeyAvailable );

            ls.Stop();
            scanner.Stop();           
        }



        static String ExceptionTrace(Exception ex)
        {
            if (ex == null)
                return "";
            return ex.Message + "\n" + ExceptionTrace(ex.InnerException);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            String sException = ExceptionTrace(e.Exception);
            MessageBox.Show("Unhandled thread exception occured:\n\n" + sException);
            Application.Exit();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            String sException = ExceptionTrace((Exception)e.ExceptionObject);
            MessageBox.Show("Unhandled exception occured:\n\n" + sException);
            Application.Exit();
        }
    }

    

}
