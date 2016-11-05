using System;
using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class LightLevelState : SensorState
    {
        [DataMember,HueLib(false,false)]
        public UInt16? lightlevel { get; set; }

        [DataMember, HueLib(false, false)]
        public bool? dark { get; set; }

        [DataMember, HueLib(false, false)]
        public bool? daylight { get; set; }
    }
}