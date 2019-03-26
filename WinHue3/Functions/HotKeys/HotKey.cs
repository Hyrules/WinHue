using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.HotKeys
{
    [DataContract, JsonConverter(typeof(HotkeyConverter))]
    public class HotKey
    {
        [DataMember]
        public ModifierKeys Modifier { get; set; }
        [DataMember]
        public Key Key { get; set; }

        [DataMember]
        public Type objecType { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public IBaseProperties properties { get; set; }

        public string Hotkey => Modifier + " + " + Key;

        [DataMember]
        public string Name { get;set; }
        [DataMember]
        public string Description { get;set; }
        [DataMember]
        public string ProgramPath { get;set; }
    }


}
