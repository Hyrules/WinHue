using System;
using System.Text.RegularExpressions;
using HueLib2;

namespace WinHue3
{
    public class DaylightView : View
    {
        private Sensor _sensor;

        #region //******************** CTOR ******************************

        public DaylightView(Sensor daylightsensor)
        {
            _sensor = daylightsensor;
            OnPropertyChanged("Longitude");
            OnPropertyChanged("Latitude");
        }

        #endregion

        #region //***************************** PROPERTIES **************************

        public string Longitude
        {
            get { return (_sensor.config).@long; }
            set
            {
                _sensor.config.@long = value;
                OnPropertyChanged();
                if (ValidateLongitude(value))
                {
                    RemoveError(GlobalStrings.Daylight_Longitude_Error);
                }
                else
                {
                    SetError(GlobalStrings.Daylight_Longitude_Error);
                }
            }
        }

        public string Latitude
        {
            get { return _sensor.config.lat; }
            set
            {
                _sensor.config.lat = value;
                OnPropertyChanged();

                if (ValidateLatitude(value))
                {
                    RemoveError(GlobalStrings.Daylight_Latitude_Error);
                }
                else
                {
                    SetError(GlobalStrings.Daylight_Latitude_Error);
                }
                
            }
        }

        public double SunriseOffset
        {
            get { return Convert.ToDouble(_sensor.config.sunriseoffset); }
            set
            {
                if(value >= -128 && value <= 127)
                    _sensor.config.sunriseoffset = Convert.ToSByte(value); OnPropertyChanged();
            }

        }

        public double SunsetOffset
        {
            get { return Convert.ToDouble(_sensor.config.sunsetoffset); }
            set
            {
                if (value >= -128 && value <= 127)
                   _sensor.config.sunsetoffset = Convert.ToSByte(value); OnPropertyChanged();
            }
        }

        public Sensor GetSensor()
        {          
            return _sensor;
        }

        #endregion

        #region //************************** METHODS ******************************************

        private static bool ValidateLatitude(string lat)
        {
            Regex reg = new Regex(@"^[0-9]{1,3}\.[0-9]{3,}[N|S]\z");
            return reg.IsMatch(lat);
        }

        private static bool ValidateLongitude(string lon)
        {
            Regex reg = new Regex(@"^[0-9]{1,3}\.[0-9]{3,}[W|E]\z");
            return reg.IsMatch(lon);
        }

        #endregion

    }
}
