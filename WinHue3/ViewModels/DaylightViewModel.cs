using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Prism.Mvvm;
using HueLib2;
using WinHue3.Models;


namespace WinHue3.ViewModels
{
    public class DaylightViewModel : BindableBase
    {
        private DaylightModel _daylight;

        public DaylightViewModel()
        {
            this.Daylight = new DaylightModel();          
            
        }

        public DaylightModel Daylight
        {
            get { return this._daylight; }
            set
            {
                SetProperty(ref _daylight, value);    
            }
        }

        public void SetDaylight(Sensor sensor)
        {
            DaylightSensorConfig state = ((DaylightSensorConfig) sensor.config);
            Daylight.SunriseOffset = (sbyte)state.sunriseoffset;
            Daylight.SunsetOffset = (sbyte)state.sunsetoffset;
            
        }


    }
}
