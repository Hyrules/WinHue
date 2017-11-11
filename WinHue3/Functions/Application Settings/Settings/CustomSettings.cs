using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.MainForm;

namespace WinHue3.Functions.Application_Settings.Settings
{
    [DataContract, Serializable]
    public class CustomSettings
    {
        public CustomSettings()
        {
            ShowHiddenScenes = false;
            Language = "en-US";
            DetectProxy = false;
            EnableDebug = true;
            UpnpTimeout = 5000;
            StartMode = 0;
            Timeout = 3000;
            ShowID = false;
            Sort = 0;
            WrapText = false;
            DefaultTT = null;
            DefaultBriGroup = 255;
            DefaultBriLight = 255;
            CheckForUpdate = true;
            CheckForBridgeUpdate = true;
        }

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
        [DataMember(EmitDefaultValue = true)]
        public bool CheckForUpdate { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public bool CheckForBridgeUpdate { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
