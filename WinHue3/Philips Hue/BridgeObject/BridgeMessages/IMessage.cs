using Newtonsoft.Json;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeMessages
{
    [JsonConverter(typeof(MessageJsonConverter))]
    public interface IMessage
    {
        
    }
}
