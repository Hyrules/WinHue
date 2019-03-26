using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [JsonObject]
    public class DataStore
    {
        public Dictionary<string, Light> lights { get; set; }

        public Dictionary<string, Group> groups { get; set; }

        public Dictionary<string, Schedule> schedules { get; set; }

        public Dictionary<string, Scene> scenes { get; set; }

        public Dictionary<string, Sensor> sensors { get; set; }

        public Dictionary<string, Rule> rules { get; set; }

        public Dictionary<string, Resourcelink> resourcelinks { get; set; }

        public BridgeSettings config { get; set; }

        public List<IHueObject> ToList()
        {
            List<IHueObject> huelist = new List<IHueObject>();
            huelist.AddRange(lights.Select(x => {x.Value.Id = x.Key;return x.Value;}).ToList());
            huelist.AddRange(groups.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList());
            huelist.AddRange(schedules.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList());
            huelist.AddRange(sensors.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList());
            huelist.AddRange(scenes.Select(x => { x.Value.Id = x.Key; return  x.Value; }).ToList());
            huelist.AddRange(rules.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList());
            huelist.AddRange(resourcelinks.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList());
            return huelist;
        }

        public override string ToString()
        {
            return Serializer.SerializeJsonObject(this);
        }
    }
}
