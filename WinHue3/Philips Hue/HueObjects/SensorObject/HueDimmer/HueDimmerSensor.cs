using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.HueDimmer
{
    [HueType("sensors")]
    public class HueDimmerSensor : ValidatableBindableBase, ISensor
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
        private HueDimmerSensorConfig _config;
        private ButtonSensorState _state;

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

        /// <summary>
        /// Config of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("Configuration of the sensor"), ExpandableObject]
        public HueDimmerSensorConfig config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        /// <summary>
        /// State of the sensor.
        /// </summary>
        [HueProperty, DataMember, Category("Sensor Properties"), Description("State of the sensor"), ExpandableObject]
        public ButtonSensorState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public ISensorConfig GetConfig()
        {
            return config;
        }

        public ISensorState GetState()
        {
            return state;
        }

        public bool SetConfig(ISensorConfig ssconfig)
        {
            if (ssconfig.GetType() != typeof(HueDimmerSensorConfig)) return false;
            config = (HueDimmerSensorConfig)ssconfig;
            return true;
        }

        public bool SetState(ISensorState sstate)
        {
            if (sstate.GetType() != typeof(ButtonSensorState)) return false;
            state = (ButtonSensorState)sstate;
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
