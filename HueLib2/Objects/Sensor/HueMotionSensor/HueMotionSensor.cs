using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class HueMotionSensor : Sensor
    {

        [DataMember, HueLib(true, true)]
        public int sensivity { get; set; }

        [DataMember, HueLib(true, true)]
        public int sensivitymax { get; internal set; }

        [DataMember, HueLib(false, false)]
        public string productid { get; internal set; }

        [DataMember, HueLib(false, false)]
        public string swconfigid { get; internal set; }
    }
}