using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.ScheduleObject
{
    /// <summary>
    /// Command of the schedule.
    /// </summary>
    [DataContract]
    public class Command
    {
        /// <summary>
        /// Put since you are modifiying a state or action.
        /// </summary>
        [DataMember]
        public string method { get; set; }
        /// <summary>
        /// // Address of the command.
        /// </summary>
        [DataMember]
        public HueAddress address { get; set; }
        /// <summary>
        /// See Body Class
        /// </summary>
        [DataMember]
        public string body { get; set; }

        /// <summary>
        /// Override to string function.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }
}
