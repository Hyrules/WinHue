using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinHue3.Philips_Hue.HueObjects
{
    [DataContract]
    public class LightSearchSerial
    {
        public LightSearchSerial()
        {
            deviceid = new List<string>();
        }

        [DataMember]
        public List<string> deviceid { get; set; }

    }
}
