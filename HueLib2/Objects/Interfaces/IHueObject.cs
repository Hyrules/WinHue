using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;

namespace HueLib2.Objects.HueObject
{
    public interface IHueObject : INotifyPropertyChanged
    {
        [JsonProperty(Required = Required.Default)]
        string Id { get; set; }
        string Name { get; set; }
        [JsonProperty(Required = Required.Default)]
        ImageSource Image { get; set; }

        object Clone();
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}
