using System;
using System.Collections.Generic;
using System.Windows.Media;
using WinHue3.Functions;

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
