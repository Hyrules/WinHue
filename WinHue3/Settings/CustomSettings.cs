using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WinHue3.Hotkeys;
using WinHue3.Models;

namespace WinHue3.Settings
{
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
            UpnpTimeout = 5000;
            StartMode = 0;
            listHotKeys = new List<HotKey>();
            Timeout = 3000;
            ShowID = false;
            Sort = 0;
            WrapText = false;
            DefaultTT = null;
            DefaultBriGroup = 255;
            DefaultBriLight = 255;
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
        public int UpnpTimeout { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int StartMode { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public List<HotKey> listHotKeys { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public ushort? AllOnTT { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public ushort? AllOffTT { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public int Timeout { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool ShowID { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public WinHueSortOrder Sort { get; set; }
        [DataMember(EmitDefaultValue =true)]
        public bool WrapText { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public ushort? DefaultTT { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public byte DefaultBriLight { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public byte DefaultBriGroup { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
