using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract,Serializable]
    public class DataStore
    {
        [DataMember(EmitDefaultValue = false,IsRequired =false)]
        public Dictionary<string,Light> lights { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Group> groups { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Schedule> schedules { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Scene> scenes { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Sensor> sensors { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Rule> rules { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Dictionary<string, Resourcelink> resourcelinks { get; set; }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
