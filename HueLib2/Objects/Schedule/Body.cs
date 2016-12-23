using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Body of the schedule.
    /// </summary>
    [DataContract, Serializable]
    public class Body
    {
        /// <summary>
        /// Brightness
        /// </summary>
        [DataMember]
        public byte? bri { get; set; }
        /// <summary>
        /// Color
        /// </summary>
        [DataMember]
        public ushort? hue { get; set; }
        /// <summary>
        /// Saturation
        /// </summary>
        [DataMember]
        public byte? sat { get; set; }
        /// <summary>
        /// Color Temp
        /// </summary>
        [DataMember]
        public ushort? ct { get; set; }
        /// <summary>
        /// Transition time
        /// </summary>
        [DataMember]
        public uint? transitiontime { get; set; }
        /// <summary>
        /// State of the light (on or off)
        /// </summary>
        [DataMember]
        public bool? on { get; set; }
        /// <summary>
        /// XY Colorspace (from 0 to 1)
        /// </summary>
        [DataMember]
        public XY xy { get; set; }
        /// <summary>
        /// Status of the schedule (enabled or disabled)
        /// </summary>
        [DataMember]
        public string status { get; set; }
       
        /// <summary>
        /// If the ended schedule autodelete itself.
        /// </summary>
        [DataMember]
        public bool? autodelete {get;set;}

        /// <summary>
        /// Scene if the schedule is linked to a scene.
        /// </summary>
        [DataMember]
        public string scene { get; set; }

        /// <summary>
        /// Flash the light if set to select or lselect.
        /// </summary>
        [DataMember]
        public string alert { get; set;}

        /// <summary>
        /// Convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }

    }
}
