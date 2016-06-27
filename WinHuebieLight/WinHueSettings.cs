using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace WinHuebieLight
{
    public class WinHueSettings
    {
        public CustomSettings settings = new CustomSettings();
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public WinHueSettings()
        {
            Reload();            
        }

        /// <summary>
        /// Save the settings to disk.
        /// </summary>
        /// <returns>True or false succeeded.</returns>
        public bool Save()
        {
            bool ret = false;
            try
            {
                string result = JsonConvert.SerializeObject(settings ,new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});
                
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!Directory.Exists(Path.Combine(path, "WinHue")))
                {
                    
                    Directory.CreateDirectory(Path.Combine(path, "WinHue"));
                
                }
                    
                File.WriteAllText(filepath, result);
                ret = true;
            }
            catch (Exception)
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
        public bool Reload()
        {
            bool result = false;
            try
            {
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!File.Exists(filepath)) return result;
                
                StreamReader sr = File.OpenText(filepath);
                
                string settingsString = sr.ReadToEnd();
                
                sr.Close();         
                
                settings =  JsonConvert.DeserializeObject<CustomSettings>(settingsString,new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                result = true;
            }
            catch (Exception)
            {
                settings = new CustomSettings();
                result = false;
                
            }
            return result;
        }
    }

    [DataContract,Serializable]
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
        }

        [DataMember]
        public Dictionary<string,BridgeSaveSettings> BridgeInfo { get; set; }
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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [DataContract,Serializable]
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
    }
}