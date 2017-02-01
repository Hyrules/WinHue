using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class LightLevelState : SensorState
    {
        [JsonIgnore]
        [DataMember]
        public UInt16? lightlevel { get; set; }

        [JsonIgnore]
        [DataMember]
        public bool? dark { get; set; }

        [JsonIgnore]
        [DataMember]
        public bool? daylight { get; set; }

    }
}