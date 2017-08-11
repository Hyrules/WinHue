using System;
using System.IO;
using Newtonsoft.Json;

namespace WinHue3.Settings
{
    public static class WinHueSettings
    {
        public static CustomSettings settings = new CustomSettings();
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static WinHueSettings()
        {
            Reload(); 
                       
        }

        /// <summary>
        /// Save the settings to disk.
        /// </summary>
        /// <returns>True or false succeeded.</returns>
        public static bool Save()
        {
            bool ret = false;
            try
            {
                string result = JsonConvert.SerializeObject(settings ,new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});
                log.Debug($@"Saving settings : {settings}");
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!Directory.Exists(Path.Combine(path, "WinHue")))
                {
                    log.Debug($@"Trying to create WinHue directory in {Path.Combine(path, "WinHue")}");
                    Directory.CreateDirectory(Path.Combine(path, "WinHue"));
                    log.Debug("Directory created succesfully.");
                }
                    
                File.WriteAllText(filepath, result);
                ret = true;
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                ret = false;
                log.Error("Error while saving the settings.",ex);

            }
            return ret;
        }

        /// <summary>
        /// Reload the settings from disk.
        /// </summary>
        /// <returns>True or false succeeded.</returns>
        public static bool Reload()
        {
            bool result = false;
            
            try
            {
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!File.Exists(filepath)) return result;
                log.Debug("Trying to open settings file...");
                StreamReader sr = File.OpenText(filepath);
                log.Debug("File open.");
                string settingsString = sr.ReadToEnd();
                log.Debug($@"Loading settings : {settingsString}");
                sr.Close();         
                log.Debug("Deserializing the settings file.");    
                settings =  JsonConvert.DeserializeObject<CustomSettings>(settingsString,new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                result = true;
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                result = false;
                log.Error("Error while loading the settings.",ex);
            }
            return result;
        }
    }



}