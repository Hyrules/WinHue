using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Application_Settings.Settings
{

    [DataContract, Serializable]
    public class BridgeSaveSettings
    {
        public BridgeSaveSettings()
        {
            ip = null;
            apikey = string.Empty;
            hiddenobjects = new List<Tuple<string, string>>();
        }

        [DataMember(IsRequired = false)]

        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress ip { get; set; }
        [DataMember(IsRequired = false)]
        public string apikey { get; set; }
        [DataMember(IsRequired = false)]
        public string name { get; set; }
        [DataMember(IsRequired = false)]
        public string clientkey { get; set; }
        [DataMember(IsRequired = false)]
        public List<Tuple<string,string>> hiddenobjects { get;set; }
    }

    public class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
           if (IPAddress.TryParse(reader.Value.ToString(), out IPAddress result))
           {
                return result;
           }
           else
           {
                throw new JsonSerializationException("Invalid IP Address");
           }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
