using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2.Objects.Scene
{
    [DataContract]
    public class SceneBody : IRuleBody
    {
        [DataMember]
        public string scene { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
