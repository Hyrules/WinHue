using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class LightLevelState : ISensorState, IRuleBody
    {
        [DataMember,ReadOnly(true)]
        public UInt16? lightlevel { get; set; }

        [DataMember, ReadOnly(true)]
        public bool? dark { get; set; }

        [DataMember, ReadOnly(true)]
        public bool? daylight { get; set; }

        [DataMember]
        public string lastupdated { get; set; }
    }
}