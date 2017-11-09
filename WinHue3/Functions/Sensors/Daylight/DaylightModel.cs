using System.ComponentModel.DataAnnotations;
using WinHue3.ViewModels;


namespace WinHue3.Models
{
    public class DaylightModel : ValidatableBindableBase
    {
        private sbyte _sunsetoffset;
        private sbyte _sunriseoffset;

        public DaylightModel()
        {
            SunriseOffset = 0;
            SunsetOffset = 0;
            
        }
          
        [RegularExpression(@"^[0-9]{1,3}\.[0-9]{3,}[W|E]\z",ErrorMessageResourceType = typeof(GlobalStrings),ErrorMessageResourceName = "Daylight_Longitude_Error")]
        public string Longitude { get; set; }

        [RegularExpression(@"^[0-9]{1,3}\.[0-9]{3,}[N|S]\z",ErrorMessageResourceType = typeof(GlobalStrings),ErrorMessageResourceName = "Daylight_Latitude_Error")]
        public string Latitude { get; set; }

        public sbyte SunriseOffset
        {
            get => _sunriseoffset;
            set => SetProperty(ref _sunriseoffset, value);
        }

        public sbyte SunsetOffset
        {
            get => _sunsetoffset;
            set
            {
                _sunsetoffset = value;
                SetProperty(ref _sunsetoffset, value);
            }
        }

    }
}