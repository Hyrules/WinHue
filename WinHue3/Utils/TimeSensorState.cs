using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
namespace WinHue3.Utils
{
    public class TimeSensorState : SensorState
    {
        public string localtime { get; set; }
        public string UTC { get; set; }
    }
}
