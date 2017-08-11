using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.ScheduleObject
{
    public class ScheduleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Schedule oldsch = (Schedule) value;
            List<PropertyInfo> prop = oldsch.GetType().GetListHueProperties();

            writer.WriteStartObject();
            foreach (PropertyInfo p in prop)
            {
                if (p.GetValue(oldsch) == null) continue;
                writer.WritePropertyName(p.Name);
                if (p.Name == "command")
                {
                    writer.WriteStartObject();
                    List<PropertyInfo> pi = oldsch.command.GetType().GetListHueProperties();
                    foreach (PropertyInfo pc in pi)
                    {
                        if(pc.GetValue(oldsch.command) == null) continue;
                        writer.WritePropertyName(pc.Name);
                        if (pc.Name == "address")
                        {
                            writer.WriteValue(oldsch.command.address.ToString());
                        }
                        else if (pc.Name == "body")
                        {
                            writer.WriteRawValue(oldsch.command.body);
                        }
                        else
                        {
                            writer.WriteValue(pc.GetValue(oldsch.command));
                        }
                        
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteValue(p.GetValue(oldsch));
                }
            }

            writer.WriteEnd();

            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            Schedule newSchedule = new Schedule();
            if(obj["name"] != null)
                newSchedule.name = obj["name"].Value<string>();
            if (obj["created"] != null)
                newSchedule.created = obj["created"].Value<string>();
            if (obj["autodelete"] != null)
                newSchedule.autodelete = obj["autodelete"].Value<bool>();
            if (obj["description"] != null)
                newSchedule.description = obj["description"].Value<string>();
            if (obj["localtime"] != null)
                newSchedule.localtime = obj["localtime"].Value<string>();
            if (obj["recycle"] != null)
                newSchedule.recycle = obj["recycle"].Value<bool>();
            if (obj["starttime"] != null)
                newSchedule.starttime = obj["starttime"].Value<string>();
            if (obj["status"] != null)
                newSchedule.status = obj["status"].Value<string>();
            if (obj["command"] == null) return newSchedule;
            newSchedule.command = new Command();
            if (obj["command"]["address"] != null)
                newSchedule.command.address = obj["command"]["address"].ToObject<HueAddress>();
            if (obj["command"]["body"] != null)
                newSchedule.command.body = JsonConvert.SerializeObject(obj["command"]["body"]);
            if (obj["command"]["method"] != null)
                newSchedule.command.method = obj["command"]["method"].Value<string>();
            return newSchedule;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Schedule) == objectType;
        }
    }
}