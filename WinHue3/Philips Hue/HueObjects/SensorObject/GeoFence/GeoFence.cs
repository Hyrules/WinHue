using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueTap;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.UnknownSensorObject
{
    [HueType("sensors")]
    public class Geofence : ValidatableBindableBase, ISensor
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
        private string _config;
        private string _state;

        /// <summary>
        /// ID of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("ID of the Sensor"), ReadOnly(true), Browsable(false), JsonIgnore]
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

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Image of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Image of the Sensor"), ReadOnly(true), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
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
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Software version of the sensor"), CreateOnly]
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
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Manufacturer name of the sensor"), CreateOnly]
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

        public string config;
        public PresenceSensorState state;

        public ISensorConfig GetConfig()
        {
            throw new NotImplementedException();
        }

        public ISensorState GetState()
        {
            throw new NotImplementedException();
        }

        public bool SetConfig(ISensorConfig sconfig)
        {
            throw new NotImplementedException();
        }

        public bool SetState(ISensorState sstate)
        {
            throw new NotImplementedException();
        }
    }

    public class SensorJsonConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
