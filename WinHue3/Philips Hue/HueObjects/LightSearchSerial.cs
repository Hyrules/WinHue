using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
