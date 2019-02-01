using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinHue3.Interface;

namespace WinHue3.Functions.Lights.SupportedDevices
{
    public static class LightImageLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, Dictionary<string,ImageSource>> _images;

        static LightImageLibrary()
        {
            _images = new Dictionary<string, Dictionary<string, ImageSource>>();
            
            _images.Add("DefaultHUE", new Dictionary<string, ImageSource>()
            {
                {"on", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_on)},
                {"off", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_off)},
                {"unr", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_unr)},
            });

            _images.Add("DefaultLIFX", new Dictionary<string, ImageSource>()
            {
                {"on", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLIFX_on)},
                {"off", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLIFX_off)},
                {"unr", GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLIFX_unr)},
            });

        }

        public static Dictionary<string, Dictionary<string, ImageSource>> Images => _images;

        public static void LoadLightsImages()
        {
            string[] listlightsfiles = Directory.GetFiles("lights", "*.png");
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            log.Info("Image path = " + path + "\\lights");
            foreach (string file in listlightsfiles)
            {
                if (!(file.Contains("_unr.png") || file.Contains("_off.png") || file.Contains("_on.png"))) continue; // CHECK IF FILE PATTERN FOR NAMING IS VALID. OTHERWISE IGNORE.
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(file); // GET FILE NAME WITHOUT EXTENSION (.png)                     
                    string filenamenostate = filename.Substring(0, filename.LastIndexOf("_")); // GET FILE NAME WITHOUT STATE SUFFIX
                    string modelORarchetype = filenamenostate;
                    if (filenamenostate.Contains("archetype_")) // CHECK IF FILE IS FOR AN ARCHETYPE, IF SO THEN REMOVE ARCHETYPE PREFIX AND ASSIGN TO VARIABLE
                    {
                        modelORarchetype = filenamenostate.Substring(filenamenostate.IndexOf("_") + 1);
                    }

                    if (!Images.ContainsKey(modelORarchetype))
                    {
                        //*** CHECK IF THE 3 FILES EXISTS OTHERWISE IGNORE***
                        if (File.Exists($"{path}\\lights\\{filenamenostate}_on.png") && File.Exists($"{path}\\lights\\{filenamenostate}_off.png") && File.Exists($"{path}\\lights\\{filenamenostate}_unr.png"))
                        {
                            log.Info($"Loading images for {filenamenostate}...");
                            Images.Add(modelORarchetype, new Dictionary<string, ImageSource>()
                            {
                                {"on" , new BitmapImage(new Uri($@"{path}\lights\{filenamenostate}_on.png" )) },
                                {"off", new BitmapImage(new Uri($@"{path}\lights\{filenamenostate}_off.png")) },
                                {"unr", new BitmapImage(new Uri($@"{path}\lights\{filenamenostate}_unr.png")) }
                            });

                        }
                        else
                        {
                            if (filenamenostate.Contains("archetype_"))
                            {
                                log.Error($"Archetype {modelORarchetype} does not have all 3 light image. Make sure you have _on _off and _unr file in your light folder.");
                            }
                            else
                            {
                                log.Error($"Model ID {modelORarchetype} does not have all 3 light image. Make sure you have _on _off and _unr file in your light folder.");
                            }

                        }

                    }
                
                }
                catch (Exception ex)
                {
                    log.Error("Error reading supported lights.", ex);
                }
            }
        }

    }
}
