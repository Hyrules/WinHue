using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
using WinHue3.Interface;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.ScheduleObject
{
    public class ScheduleJsonConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Schedule oldsch = (Schedule) value;
            List<PropertyInfo> prop = oldsch.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();

            writer.WriteStartObject();
            foreach (PropertyInfo p in prop)
            {
                if (p.GetValue(oldsch) == null) continue;
                if (p.GetCustomAttributes(typeof(JsonIgnoreAttribute)).Count() == 1) continue;
                writer.WritePropertyName(p.Name);
                if (p.Name == "command")
                {
                    writer.WriteStartObject();
                    List<PropertyInfo> pi = oldsch.command.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList(); ;
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
            serializer.DateParseHandling = DateParseHandling.None;
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializer.DateFormatString = "yyyy-MM-dd";
            reader.DateFormatString = "yyyy-MM-dd";
            reader.DateParseHandling = DateParseHandling.None;
      
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
            {
                newSchedule.localtime = obj["localtime"].Value<string>();
                if (newSchedule.localtime.Contains(" ")) newSchedule.localtime = newSchedule.localtime.Replace(" ", "T"); // Bypass a stupid function of JSON.Net that parses dates
            }
            else
            {
                if(obj["time"] != null)
                    newSchedule.localtime = obj["time"]?.Value<string>();
            }
                
               
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

            ImageSource imgsource;
            if (newSchedule.localtime.Contains("PT"))
            {
                imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.timer_clock);
            }
            else if (newSchedule.localtime.Contains("W"))
            {
                imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.stock_alarm);
            }
            else if (newSchedule.localtime.Contains("T"))
            {
                imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.SchedulesLarge);
            }
            else
            {
                imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.schedules);
            }

            newSchedule.Image = imgsource;

            return newSchedule;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Schedule) == objectType;
        }
    }
}