using System.Windows.Media;

namespace WinHue3.LIFX.WinHue
{
    public interface ILifxObject
    {
        string Name { get; }
        ImageSource Image { get ; set; }

    }
}
