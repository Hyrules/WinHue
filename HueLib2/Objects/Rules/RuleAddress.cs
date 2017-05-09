using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.DataGrid.Converters;

namespace HueLib2.Objects.Rules
{
    [DataContract, JsonConverter(typeof(RuleAddressJsonConverter))]
    public class RuleAddress
    {
        public RuleAddress()
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
         * 
         ************************************/


        public string objecttype { get; set; }
        public string id { get; set; }
        public string property { get; set; }
        public string subprop { get; set; }

        public RuleAddress(string address)
        {
            string[] parts = address.Split('/');
            objecttype = parts[1];
            if (parts.Length == 3)
            {
                property = parts[2];
                id = string.Empty;
            }
            else
            {
                id = parts[2];
                property = parts[3];
            }
                
            subprop = parts.Length == 5 ? parts[4] : string.Empty;
        }

        public override string ToString()
        {
            string newid = id != string.Empty ? $"/{id}" : id;
            string newprop = property != string.Empty ? $"/{property}" : property;
            string newsubprop = subprop != string.Empty ? $"/{subprop}" : subprop;

            return $"/{objecttype}" + newid + newprop + newsubprop; 
        }

        public override bool Equals(object obj)
        {
            RuleAddress ra = obj as RuleAddress;
            if ((object)ra == null) return false;
            if (obj.GetType() != typeof(RuleAddress)) return false;
            return this.ToString() == obj.ToString();
        }

        public static bool operator ==(RuleAddress objA, RuleAddress objB)
        {
            if (ReferenceEquals(objA, objB))
            {
                return true;
            }

            if ((object) objA == null || (object) objB == null)
            {
                return false;
            }

            return objA.objecttype == objB.objecttype && objA.id == objB.id && objA.property == objB.property &&
                   objA.subprop == objB.subprop;
        }

        public static bool operator !=(RuleAddress objA, RuleAddress objB)
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

    public class RuleAddressJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RuleAddress ruleAddress = (RuleAddress) value;
            writer.WriteValue(ruleAddress.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            RuleAddress ruleAddress = new RuleAddress();
            
            string[] str = reader.Value.ToString().Split('/');
            ruleAddress.objecttype = str[1];
            if (str.Length == 5)
            {
                ruleAddress.id = str[2];
                ruleAddress.property = str[3];
                ruleAddress.subprop = str[4];
            }
            else if(str.Length == 4)
            {
                ruleAddress.id = str[2];
                ruleAddress.property = str[3];
                
            }
            else
            {
                ruleAddress.property = str[2];
            }
            return ruleAddress;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RuleAddress);
        }

        
    }
}
