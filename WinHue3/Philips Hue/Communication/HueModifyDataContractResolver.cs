using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.Communication
{
    public class HueModifyDataContractResolver : DefaultContractResolver
    {


        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> filtered = new List<JsonProperty>();
            IList<JsonProperty> unfiltered = base.CreateProperties(type, memberSerialization);
            foreach (JsonProperty p in unfiltered)
            {
                if (p.AttributeProvider.GetAttributes(typeof(JsonIgnoreAttribute), false).Count == 1) continue;
                if (p.AttributeProvider.GetAttributes(typeof(CreateOnlyAttribute),false).Count == 1) continue;
                // DONT SERIALIZE THOSE PROPERTIES
                if (p.AttributeProvider.GetAttributes(typeof(DontSerialize), false).Count == 1) continue;
                filtered.Add(p);
            }

            return filtered;
        }
    }
}
