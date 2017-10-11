using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;
using Newtonsoft.Json.Linq;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public class UnknownSensorConfig : ValidatableBindableBase, ISensorConfigBase
    {
        public dynamic value { get; set; }
    }
}
