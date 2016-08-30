using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using HueLib2;

namespace WinHue3
{
    [DataContract]
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
        public CommonProperties properties { get; set; }

        public string Hotkey => Modifier + " + " + Key;

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }

    }
}
