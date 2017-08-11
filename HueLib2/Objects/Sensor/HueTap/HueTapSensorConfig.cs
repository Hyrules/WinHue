using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class HueTapSensorConfig : ISensorConfig
    {


        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember]
        public bool? on { get; set; }

    }
}
