using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static WinHue3.Models.MainFormModel;

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
            LiveSliders = false;
            UpnpTimeout = 5000;
            DelayLiveSliders = 25;
            StartWithWindows = false;
            StartMode = 0;
            listHotKeys = new List<HotKey>();
            Timeout = 3000;
            ShowID = false;
            Sort = 0;
            WrapText = TextWrapping.NoWrap;
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
        [DataMember(EmitDefaultValue = true)]
        public bool ShowID { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public WinHueSortOrder Sort { get; set; }
        [DataMember(EmitDefaultValue =true)]
        public TextWrapping WrapText { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
