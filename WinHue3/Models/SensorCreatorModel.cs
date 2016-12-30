using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3.Models
{
    public class SensorCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private string _modelId;
        private string _swversion;
        private string _mfgname;
        private string _uniqueid;
        private string _url;
        private string _type;
        private bool _on;
        private SensorConfig _config;

        public SensorCreatorModel()
        {
            Name = string.Empty;
            Mfgname = string.Empty;
            ModelId = string.Empty;
            Uniqueid = string.Empty;
            Swversion = string.Empty;
            Type = "CLIPGenericFlag";
        }


        [StringLength(32,MinimumLength = 1, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Sensor_EmptyField_Error")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [StringLength(32,MinimumLength = 6, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Sensor_FieldTooShort")]
        public string ModelId
        {
            get { return _modelId; }
            set { SetProperty(ref _modelId, value); }
        }

        [StringLength(16,MinimumLength = 1, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Sensor_EmptyField_Error")]
        public string Swversion
        {
            get { return _swversion; }
            set { SetProperty(ref _swversion, value); }
        }

        [StringLength(32,MinimumLength = 6,ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Sensor_FieldTooShort")]
        public string Mfgname
        {
            get { return _mfgname; }
            set { SetProperty(ref _mfgname, value); }
        }

        [StringLength(32,MinimumLength=6, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Sensor_FieldTooShort")]
        public string Uniqueid
        {
            get { return _uniqueid; }
            set { SetProperty(ref _uniqueid, value); }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                SetProperty(ref _type,value);
                switch (_type)
                {
                    case "CLIPGenericFlag":
                        Config = new ClipGenericFlagSensorConfig() {on=true};
                        break;
                    case "CLIPGenericStatus":
                        Config = new ClipGenericStatusSensorConfig() { on = true };
                        break;
                    case "CLIPHumidity":
                        Config = new ClipHumiditySensorConfig() { on = true };
                        break;
                    case "CLIPOpenClose":
                        Config = new ClipOpenCloseSensorConfig() { on = true };
                        break;
                    case "CLIPPresence":
                        Config = new ClipPresenceSensorConfig() { on = true };
                        break;
                    case "CLIPTemperature":
                        Config = new TemperatureSensorConfig() { on = true };
                        break;
                    case "CLIPSwitch":
                        Config = null;
                        break;
                    default:
                        Config = new ClipGenericFlagSensorConfig() { on = true };
                        break;
                }
            }
        }

        public SensorConfig Config
        {
            get { return _config; }
            set { SetProperty(ref _config,value); }
        }
    }
}
