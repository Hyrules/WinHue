using System.Collections.Generic;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    /// <summary>
    /// Class for the software update.
    /// </summary>
    [DataContract]
    public class SwUpdate
    {
        /// <summary>
        /// State of the update  
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int? updatestate { get; set; }
        /// <summary>
        /// url of the update
        /// </summary>
        [DataMember(IsRequired = false)]
        public string url { get; set; }
        /// <summary>
        /// Message of the update
        /// </summary>
        [DataMember(IsRequired = false)]
        public string text { get; set; }
        /// <summary>
        /// // Notify for the update
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public bool notify { get; set; }

        /// <summary>
        /// Check for update.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public bool checkforupdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Devicetypes devicetypes { get; set; }
    }

    /// <summary>
    /// devicetype
    /// </summary>
    [DataContract]
    public class Devicetypes
    {
        /// <summary>
        /// Type of the update.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public bool? bridge { get; set; }
        /// <summary>
        /// Lights to update.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string> lights { get; set; }

        /// <summary>
        /// Sensors to update.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string> sensors { get; set; }
    }
}
