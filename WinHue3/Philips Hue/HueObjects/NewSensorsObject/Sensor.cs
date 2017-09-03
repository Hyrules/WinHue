using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueTap;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    [DataContract, JsonConverter(typeof(SensorJsonConverter))]
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
        private SensorConfigBase _config;
        private SensorStateBase _state;

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("ID of the Sensor"), ReadOnly(true), Browsable(false),
         JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Name of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Name of the sensor")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Image of the Sensor"), ReadOnly(true),
         Browsable(false), JsonIgnore]
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
        [HueProperty, DataMember, Category("Sensor Properties"), Description("ModelID of the sensor"), CreateOnly]
        public string modelid
        {
            get => _modelid;
            set => SetProperty(ref _modelid, value);
        }

        /// <summary>
        /// Software version of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Software version of the sensor"),
         CreateOnly]
        public string swversion
        {
            get => _swversion;
            set => SetProperty(ref _swversion, value);
        }

        /// <summary>
        /// Type of sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Type of the sensor"), CreateOnly]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        /// <summary>
        /// Manufacturer name of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Manufacturer name of the sensor"),
         CreateOnly]
        public string manufacturername
        {
            get => _manufacturername;
            set => SetProperty(ref _manufacturername, value);
        }

        /// <summary>
        /// Unique ID of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Unique ID of the sensor"), CreateOnly]
        public string uniqueid
        {
            get => _uniqueid;
            set => SetProperty(ref _uniqueid, value);
        }

        /// <summary>
        /// Product ID of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Product ID of the sensor")]
        public string productid
        {
            get => _productid;
            set => SetProperty(ref _productid, value);
        }


        /// <summary>
        /// Software Config ID of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Software configuration ID of the sensor")]
        public string swconfigid
        {
            get => _swconfigid;
            set => SetProperty(ref _swconfigid, value);
        }

        /// <summary>
        /// Config of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Configuration of the sensor"),
         ExpandableObject, CreateOnly]
        public SensorConfigBase config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public T GetConfig<T>() where T : SensorConfigBase
        {
            return (T) _config;
        }

        /// <summary>
        /// State of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Configuration of the sensor"),
         ExpandableObject, ReadOnly(true)]
        public SensorStateBase state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public T GetState<T>() where T : SensorStateBase
        {
            return (T) _state;
        }

        /*public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }*/
    }

    public class SensorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            Sensor sensor = new Sensor();
            JObject obj = serializer.Deserialize<JObject>(reader);

            sensor.type = obj["type"]?.Value<string>();
            sensor.name = obj["name"]?.Value<string>();
            sensor.manufacturername = obj["manufacturername"]?.Value<string>();
            sensor.modelid = obj["modelid"]?.Value<string>();
            sensor.swconfigid = obj["swconfigid"]?.Value<string>();
            sensor.swversion = obj["swversion"]?.Value<string>();
            sensor.config = TryConvertConfig(obj["config"]);
            sensor.state = TryConvertState(obj["state"]);
            return sensor;
        }

        private SensorConfigBase TryConvertConfig(JToken config)
        {



            try
            {
                return config.ToObject<ClipGenericFlagSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<ClipGenericStatusSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<ClipHumiditySensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<ClipOpenCloseSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<ClipPresenceSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<LightLevelConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<TemperatureSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<DaylightSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<HueDimmerSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<HueMotionSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return config.ToObject<HueTapSensorConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            return new UnknownSensorConfig() {value = config.Value<string>()};
        }

        private SensorStateBase TryConvertState(JToken state)
        {
            try
            {
                return state.ToObject<ClipGenericFlagSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<ClipGenericStatusSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<ClipHumiditySensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<ClipOpenCloseSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<PresenceSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<LightLevelState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<TemperatureSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<DaylightSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                return state.ToObject<ButtonSensorState>();
            }
            catch (Exception)
            {
                // ignored
            }

            return new UnknownSensorState(){value = state.Value<string>()};
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Sensor);
        }
    }
}