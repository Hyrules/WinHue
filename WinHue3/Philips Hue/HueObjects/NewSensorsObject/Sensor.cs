using System;
using System.ComponentModel;
using System.Dynamic;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.GeoFence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueTap;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using WinHue3.Interface;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    [JsonObject, JsonConverter(typeof(SensorJsonConverter))]
    public class Sensor : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private string _id;
        private ImageSource _image;
        private string _modelid;
        private string _swversion;
        private string _type;
        private string _manufacturername;
        private string _uniqueid;
        private string _productid;
        private string _swconfigid;
        private dynamic _config;

        private dynamic _state;

        //public SwUpdate _swupdate;
        private bool _visible;

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [Category("Sensor Properties"), Description("ID of the Sensor"), Browsable(false), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Name of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Name of the sensor")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [Category("Sensor Properties"), Description("Image of the Sensor"), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Model id of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("ModelID of the sensor"), CreateOnly]
        public string modelid
        {
            get => _modelid;
            set => SetProperty(ref _modelid, value);
        }

        /// <summary>
        /// Software version of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Software version of the sensor"), CreateOnly]
        public string swversion
        {
            get => _swversion;
            set => SetProperty(ref _swversion, value);
        }

        /// <summary>
        /// Type of sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Type of the sensor"), CreateOnly]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        /// <summary>
        /// Manufacturer name of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Manufacturer name of the sensor"), CreateOnly]
        public string manufacturername
        {
            get => _manufacturername;
            set => SetProperty(ref _manufacturername, value);
        }

        /// <summary>
        /// Unique ID of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Unique ID of the sensor"), CreateOnly]
        public string uniqueid
        {
            get => _uniqueid;
            set => SetProperty(ref _uniqueid, value);
        }

        /// <summary>
        /// Product ID of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Product ID of the sensor"), CreateOnly]
        public string productid
        {
            get => _productid;
            set => SetProperty(ref _productid, value);
        }


        /// <summary>
        /// Software Config ID of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Software configuration ID of the sensor"), CreateOnly]
        public string swconfigid
        {
            get => _swconfigid;
            set => SetProperty(ref _swconfigid, value);
        }

        /// <summary>
        /// Config of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Configuration of the sensor"), ExpandableObject, JsonIgnore]
        public ISensorConfigBase config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public T GetConfig<T>() where T : ISensorConfigBase
        {
            return (T) _config;
        }

        /// <summary>
        /// State of the sensor.
        /// </summary>
        [Category("Sensor Properties"), Description("Configuration of the sensor"), ExpandableObject, ReadOnly(true)]
        public ISensorStateBase state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        /*[Category("Sensor Properties"), Description("Sensor Update"),ExpandableObject, ReadOnly(true)]
        public SwUpdate swupdate
        {
            get => _swupdate;
            set => SetProperty(ref _swupdate, value);
        }*/

        public override bool Equals(object obj)
        {
            return obj is Sensor hueobject && hueobject.Id == Id;
        }

        [JsonIgnore, Browsable(false)]
        public bool visible
        {
            get { return _visible; }
            set { SetProperty(ref _visible, value); }
        }

        public T GetState<T>() where T : ISensorStateBase
        {
            return (T) _state;
        }

        public override string ToString()
        {
            return name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class StateJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class SensorJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Sensor sensor = new Sensor();
            JObject obj = serializer.Deserialize<JObject>(reader);

            sensor.type = obj["type"]?.Value<string>();
            sensor.name = obj["name"]?.Value<string>();
            sensor.manufacturername = obj["manufacturername"]?.Value<string>();
            sensor.modelid = obj["modelid"]?.Value<string>();
            sensor.swconfigid = obj["swconfigid"]?.Value<string>();
            sensor.swversion = obj["swversion"]?.Value<string>();
            sensor.config = TryConvertConfig(obj["config"], sensor.type);
            sensor.state = TryConvertState(obj["state"], sensor.type);
            sensor.uniqueid = obj["uniqueid"]?.Value<string>();

            if (sensor.type != null)
            {
                switch (sensor.type)
                {
                    case "ZGPSwitch":
                        sensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.huetap);
                        break;
                    case "ZLLSwitch":
                        sensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.dimmer);
                        break;
                    case "ZLLPresence":
                        sensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.Motion);
                        break;
                    default:
                        sensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.sensor);
                        break;

                }
            }
                

            return sensor;
        }

        private ISensorConfigBase TryConvertConfig(JToken config, string type)
        {
            switch (type)
            {
                case "CLIPGenericFlag":
                    return config.ToObject<ClipGenericFlagSensorConfig>();
                case "CLIPGenericStatus":
                    return config.ToObject<ClipGenericStatusSensorConfig>();
                case "CLIPHumidity":
                    return config.ToObject<ClipHumiditySensorConfig>();
                case "CLIPOpenClose":
                    return config.ToObject<ClipOpenCloseSensorConfig>();
                case "CLIPPresence":
                    return config.ToObject<ClipPresenceSensorConfig>();
                case "ZLLTemperature":
                case "CLIPTemperature":
                    return config.ToObject<TemperatureSensorConfig>();
                case "CLIPLightLevel":
                case "ZLLLightLevel":
                    return config.ToObject<LightLevelConfig>();
                case "CLIPSwitch":
                case "ZGPSwitch":
                    return config.ToObject<HueTapSensorConfig>();
                case "ZLLSwitch":
                    return config.ToObject<HueDimmerSensorConfig>();
                case "ZLLPresence":
                    return config.ToObject<HueMotionSensorConfig>();
                case "Daylight":
                    return config.ToObject<DaylightSensorConfig>();
                case "Geofence":
                    return config.ToObject<GeofenceConfig>();
                default:
                    return new UnknownSensorConfig() {value = config.ToObject<ExpandoObject>()};
            }
        }

        private ISensorStateBase TryConvertState(JToken state, string type)
        {
            switch (type)
            {
                case "CLIPGenericFlag":
                    return state.ToObject<ClipGenericFlagSensorState>();
                case "CLIPGenericStatus":
                    return state.ToObject<ClipGenericStatusSensorState>();
                case "CLIPHumidity":
                    return state.ToObject<ClipHumiditySensorState>();
                case "CLIPOpenClose":
                    return state.ToObject<ClipOpenCloseSensorState>();
                case "CLIPPresence":
                case "Geofence":
                case "ZLLPresence":
                    return state.ToObject<PresenceSensorState>();
                case "ZLLTemperature":
                case "CLIPTemperature":
                    return state.ToObject<TemperatureSensorState>();
                case "CLIPLightLevel":
                case "ZLLLightLevel":
                    return state.ToObject<LightLevelState>();
                case "CLIPSwitch":
                case "ZGPSwitch":
                case "ZLLSwitch":
                    return state.ToObject<ButtonSensorState>();
                case "Daylight":
                    return state.ToObject<DaylightSensorState>();

                default:
                    return new UnknownSensorState() {value = state.ToObject<ExpandoObject>()};
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Sensor);
        }
    }
}