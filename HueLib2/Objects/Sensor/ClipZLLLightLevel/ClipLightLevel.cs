using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class LightLevel : Sensor
    {
        [DataMember,HueLib(false,false)]
        public uint? lightlevel { get; set; }

        [DataMember, HueLib(false,false)]
        public bool? dark { get; set; }

        [DataMember, HueLib(false,false)]
        public bool? daylight { get; set; }


    }
}
