using Newtonsoft.Json;

namespace WinHue3.Philips_Hue_2.BridgeMessages
{
    [JsonConverter(typeof(MessageJsonConverter))]
    public interface IMessage
    {
        
    }
}
