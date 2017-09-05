using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public abstract class SensorStateBase : ValidatableBindableBase
    {

      /*  public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }*/
    }
}
