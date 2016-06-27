using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib_base;
using WinHue3.Resources;

namespace WinHue3
{
    public class SensorCreatorView : View
    {
        private Sensor _sensor;
        private string url;

        #region CTOR

        public SensorCreatorView()
        {
            _sensor = new Sensor();
            _sensor.config = new SensorConfig();
            _sensor.state = new ClipGenericFlagSensorState();
            _sensor.type = "CLIPGenericFlag";
            _sensor.config.on = true;
            SetError(GlobalStrings.Sensor_EmptyField_Error, "ManufacturerName");
            SetError(GlobalStrings.Sensor_EmptyField_Error, "UniqueID");
            SetError(GlobalStrings.Sensor_EmptyField_Error, "Name");
            SetError(GlobalStrings.Sensor_EmptyField_Error, "SwVersion");
            SetError(GlobalStrings.Sensor_EmptyField_Error, "ModelID");
            SetError(GlobalStrings.Sensor_EmptyField_Error, "Url");
        }

        public SensorCreatorView(Sensor sensor)
        {
            _sensor = sensor;
            Errors.Clear();
        }

        #endregion

        #region PROPERTIES

        public bool isEditing
        {
            get { return _sensor.Id == null; }
        }

        public string CreateBtnText
        {
            get
            {
                return _sensor.Id == null ? GUI.SensorCreatorForm_CreateButton : GUI.SensorCreatorForm_Update;
            }

        }

        public int Type
        {
            get
            {
                int index = 0;
                switch(_sensor.type)
                {
                    case "CLIPGenericFlag":
                        index = 0;
                        break;
                    case "CLIPGenericStatus":
                        index = 1;
                        break;
                    case "CLIPHumidity":
                        index = 2;
                        break;
                    case "CLIPOpenClose":
                        index = 3;
                        break;
                    case "CLIPPresence":
                        index = 4;
                        break;
                    case "CLIPTemperature":
                        index = 5;
                        break;
                    default:
                        index = 0;
                        break;
                }

                return index;
            }
            set
            {
                if(_sensor.config.HasProperty("url"))
                {
                    url = _sensor.config.GetType().GetProperty("url").GetValue(_sensor.config).ToString();
                }
                _sensor.config = new SensorConfig();

                switch (value)
                {
                    case 0:
                        _sensor.type = "CLIPGenericFlag";
                        _sensor.state = new ClipGenericFlagSensorState();
                        break;
                    case 1:
                        _sensor.type = "CLIPGenericStatus";
                        _sensor.state = new ClipGenericStatusState();
                        break;
                    case 2:
                        _sensor.type = "CLIPHumidity";
                        _sensor.state = new ClipHumiditySensorState();
                        break;
                    case 3:
                        _sensor.type = "CLIPOpenClose";
                        _sensor.state = new ClipOpenCloseSensorState();
                        break;
                    case 4:
                        _sensor.type = "CLIPPresence";
                        _sensor.state = new ClipPresenceSensorState();
                        break;
                    case 5:
                        _sensor.type = "CLIPTemperature";
                        _sensor.state = new ClipTemperatureSensorState();
                        break;
                    default:
                        _sensor.type = "CLIPGenericFlag";
                        _sensor.state = new ClipGenericFlagSensorState();
                        break;
                }

                if (_sensor.config.HasProperty("url"))
                {
                    _sensor.config.GetType().GetProperty("url").SetValue(_sensor.config, url);
                }

                OnPropertyChanged();
            }
        }

        public bool HasUrl => _sensor.config.HasProperty("url");

        public string Url
        {
            get
            {
                if(_sensor.config.HasProperty("url"))
                {
                    if(_sensor.config.GetType().GetProperty("url").GetValue(_sensor.config) == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return _sensor.config.GetType().GetProperty("url").GetValue(_sensor.config).ToString();
                    }
                 }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                _sensor.config.GetType().GetProperty("url").SetValue(_sensor.config, value);
                OnPropertyChanged();
                OnPropertyChanged("HasUrl");
                if (_sensor.config.HasProperty("url"))
                {
                    if (value.IsValid())
                    {
                        if (Uri.IsWellFormedUriString(value,UriKind.Absolute))
                        {
                            RemoveError(GlobalStrings.Sensor_InvalidURL);
                            RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                        }
                        else
                        {
                            SetError(GlobalStrings.Sensor_InvalidURL);
                        }
                    }
                    else
                    {
                        SetError(GlobalStrings.Sensor_EmptyField_Error);
                    }
                }

            }
        }


        public bool On
        {
            get
            {
                return _sensor.config.on ?? false;
            }

            set
            {
                _sensor.config.on = value;
            }
        }


        public string ManufacturerName
        {
            get { return _sensor.manufacturername ?? string.Empty ; }
            set
            {
                _sensor.manufacturername = value;
                OnPropertyChanged();

                if (value.IsValid())
                {
                    RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                }
                else
                {
                    SetError(GlobalStrings.Sensor_EmptyField_Error);
                }

                if (value.Length < 6)
                {
                    SetError(GlobalStrings.Sensor_FieldTooShort);
                }
                else
                {
                    RemoveError(GlobalStrings.Sensor_FieldTooShort);
                }
            }
        }

        public string UniqueID
        {
            get
            {
                return _sensor.uniqueid ?? string.Empty;
            }

            set
            {
                _sensor.uniqueid = value;
                OnPropertyChanged();

                if (value.IsValid())
                {

                    RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                }
                else
                {
                    SetError(GlobalStrings.Sensor_EmptyField_Error);
                }

                if (value.Length < 6)
                {
                    SetError(GlobalStrings.Sensor_FieldTooShort);
                }
                else
                {
                    RemoveError(GlobalStrings.Sensor_FieldTooShort);
                }
            }
        }

        public string Name
        {
            get { return _sensor.name ?? string.Empty; }
            set
            {
                _sensor.name = value;
                OnPropertyChanged();

                if (value.IsValid())
                {

                    RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                }
                else
                {
                    SetError(GlobalStrings.Sensor_EmptyField_Error);
                }
            }
        }

        public string ModelID
        {
            get { return _sensor.modelid ?? string.Empty; }
            set
            {
                _sensor.modelid = value;
                OnPropertyChanged();

                if (value.IsValid())
                {
                    RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                }
                else
                {
                    SetError(GlobalStrings.Sensor_EmptyField_Error);
                }

                if (value.Length < 6)
                {
                    SetError(GlobalStrings.Sensor_FieldTooShort);
                }
                else
                {
                    RemoveError(GlobalStrings.Sensor_FieldTooShort);
                }
            }
        }

        public string SwVersion
        {
            get
            {
                return _sensor.swversion ?? string.Empty;
            }

            set
            {
                _sensor.swversion = value;
                OnPropertyChanged();

                if (value.IsValid())
                {
                    RemoveError(GlobalStrings.Sensor_EmptyField_Error);
                }
                else
                {
                    SetError(GlobalStrings.Sensor_EmptyField_Error);
                }
            }
        }

        #endregion

        #region METHODS

        public Sensor GetSensor()
        {
            return _sensor;
        }

        #endregion
    }
}
