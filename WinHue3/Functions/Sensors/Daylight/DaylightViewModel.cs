using Prism.Mvvm;
using WinHue3.Models;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;


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
            get => this._daylight;
            set => SetProperty(ref _daylight, value);
        }

        public void SetDaylight(Sensor sensor)
        {
            DaylightSensorConfig config = (DaylightSensorConfig)sensor.config;
            Daylight.SunriseOffset = (sbyte)config.sunriseoffset;
            Daylight.SunsetOffset = (sbyte)config.sunsetoffset;
            
        }


    }
}
