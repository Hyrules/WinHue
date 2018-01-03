using System.Windows.Media;

namespace WinHue3.Philips_Hue.HueObjects.Common
{
    public interface IHueObject
    {
        string Id { get; set; }
        ImageSource Image { get; set; }
        string name { get; set; }
        object Clone();
        bool visible { get; set; }
}
}
