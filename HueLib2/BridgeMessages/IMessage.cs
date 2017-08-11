using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2.BridgeMessages
{
    [JsonConverter(typeof(MessageJsonConverter))]
    public interface IMessage
    {
        
    }
}
