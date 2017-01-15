using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using WinHue3;

namespace whc
{
    public static class WinHueSettings
    {
        public static CustomSettings settings = new CustomSettings();
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        

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
                string result = JsonConvert.SerializeObject(settings, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!Directory.Exists(Path.Combine(path, "WinHue")))
                {

                    Directory.CreateDirectory(Path.Combine(path, "WinHue"));

                }

                File.WriteAllText(filepath, result);
                ret = true;
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                ret = false;
  

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

                StreamReader sr = File.OpenText(filepath);

                string settingsString = sr.ReadToEnd();

                sr.Close();

                settings = JsonConvert.DeserializeObject<CustomSettings>(settingsString, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                result = true;
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                result = false;

            }
            return result;
        }
    }

    [DataContract, Serializable]
    public class CustomSettings
    {
        public CustomSettings()
        {
            BridgeInfo = new Dictionary<string, BridgeSaveSettings>();
            DefaultBridge = string.Empty;
            ShowHiddenScenes = false;
            Language = "en-US";
            DetectProxy = false;
            EnableDebug = true;
            LiveSliders = false;
            UpnpTimeout = 5000;
            DelayLiveSliders = 25;
            StartWithWindows = false;
            StartMode = 0;
            listHotKeys = new List<HotKey>();
            Timeout = 3000;
        }

        [DataMember]
        public Dictionary<string, BridgeSaveSettings> BridgeInfo { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string DefaultBridge { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool ShowHiddenScenes { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string Language { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool DetectProxy { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool EnableDebug { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool LiveSliders { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int UpnpTimeout { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int DelayLiveSliders { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool StartWithWindows { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int StartMode { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public List<HotKey> listHotKeys { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public uint? AllOnTT { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public uint? AllOffTT { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int Timeout { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [DataContract, Serializable]
    public class BridgeSaveSettings
    {
        public BridgeSaveSettings()
        {
            ip = string.Empty;
            apikey = string.Empty;
        }

        [DataMember(IsRequired = false)]
        public string ip { get; set; }
        [DataMember(IsRequired = false)]
        public string apikey { get; set; }
        [DataMember(IsRequired = false)]
        public string apiversion { get; set; }
        [DataMember(IsRequired = false)]
        public string swversion { get; set; }
        [DataMember(IsRequired = false)]
        public string name { get; set; }
    }
}