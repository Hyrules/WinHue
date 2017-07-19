using System.ComponentModel;
using System.Windows.Media;

namespace HueLib2.Objects.HueObject
{
    public interface IHueObject : INotifyPropertyChanged
    {
        string Id { get; set; }
        string Name { get; set; }
        ImageSource Image { get; set; }

        object Clone();
    }
}
