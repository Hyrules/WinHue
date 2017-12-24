using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Utils;

namespace WinHue3.Functions.Sensors.Daylight
{
    public class DaylightViewModel : ValidatableBindableBase
    {
        private DaylightModel _daylight;

        public DaylightViewModel()
        {
            _daylight = new DaylightModel();          
            
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
