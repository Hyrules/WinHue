using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace WinHue3.Philips_Hue.HueObjects.Common
{
    [DataContract, JsonConverter(typeof(HueAddressJsonConverter))]
    public class HueAddress
    {
        public HueAddress()
        {
            objecttype = string.Empty;
            id = string.Empty;
            property = string.Empty;
            subprop = string.Empty;
        }
        // POSSIBLE TYPES OF STRINGS
        /************************************
         * 1) /objectype/id/property/subprop
         * 2) /objectype/property
         * 3) /objectype/id/property
         * 4) /api/key/objectype/id/property/subprop
         * 5) /api/key/objectype/id/property
         ************************************/

        public string api { get; set; }
        public string key { get; set; }
        public string objecttype { get; set; }
        public string id { get; set; }
        public string property { get; set; }
        public string subprop { get; set; }

        public HueAddress(string address)
        {
            List<string> parts = address.Split('/').ToList();
            parts.RemoveAt(0); // remove the blank first item

            if (parts.Contains("api"))
            {
                api = parts[0];
                key = parts[1];
                //remove the api
                parts.Remove("api");
                //remove the apikey
                parts.RemoveAt(0);
            }

            objecttype = parts[0];

            if (parts.Count == 2)
            {
                property = parts[1];
                id = string.Empty;
            }
            else
            {
                id = parts[1];
                property = parts[2];
            }

            subprop = parts.Count == 4 ? parts[3] : string.Empty;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (api != null)
            {
                sb.Append($"/api/{key}");
            }

            if (objecttype != string.Empty)
            {
                sb.Append($"/{objecttype}");
            }

            if (id != string.Empty)
            {
                sb.Append($"/{id}");
            }

            if (property != string.Empty)
            {
                sb.Append($"/{property}");
            }

            if (subprop != string.Empty)
            {
                sb.Append($"/{subprop}");
            }


            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            HueAddress ra = obj as HueAddress;
            if ((object)ra == null) return false;
            if (obj.GetType() != typeof(HueAddress)) return false;
            return this.ToString() == obj.ToString();
        }

        public static bool operator ==(HueAddress objA, HueAddress objB)
        {
            if (ReferenceEquals(objA, objB))
            {
                return true;
            }

            if ((object)objA == null || (object)objB == null)
            {
                return false;
            }

            return objA.objecttype == objB.objecttype && objA.id == objB.id && objA.property == objB.property &&
                   objA.subprop == objB.subprop;
        }

        public static bool operator !=(HueAddress objA, HueAddress objB)
        {
            if (objA == null && objB == null) return true;
            if (objA == null || objB == null) return false;
            return objA.ToString() != objB.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }

    public class HueAddressJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            HueAddress HueAddress = (HueAddress)value;
            writer.WriteValue(HueAddress.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HueAddress hueAddress = new HueAddress();

            List<string> str = reader.Value.ToString().Split('/').ToList();
            str.RemoveAt(0);

            if (str.Contains("api"))
            {
                hueAddress.api = "api";
                hueAddress.key = str[1];
                //remove the api
                str.Remove("api");
                //remove the apikey
                str.RemoveAt(0);
            }

            hueAddress.objecttype = str[0];

            if (str.Count == 4)
            {
                hueAddress.id = str[1];
                hueAddress.property = str[2];
                hueAddress.subprop = str[3];
                
            }
            else if (str.Count == 3)
            {
                hueAddress.id = str[1];
                hueAddress.property = str[2];

            }
            else if(str.Count == 2)
            {
                hueAddress.property = str[1];
            }
            return hueAddress;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HueAddress);
        }


    }
}
