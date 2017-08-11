using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HueLib2.Objects.Interfaces;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2.Objects.Sensor.HueMotionSensor
{
    [HueType("sensors")]
    public class HueMotionSensor : ISensor
    {
        private string _name;
        private string _id;
        private ImageSource _image;

        public event PropertyChangedEventHandler PropertyChanged;
        [JsonProperty(Required = Required.Default)]
        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string modelid { get; set; }
        public string swversion { get; set; }
        public string type { get; set; }
        public string manufacturername { get; set; }
        public string uniqueid { get; set; }
        [DataMember, ExpandableObject, Category("Configuration"), Description("Configuration of the sensor"), CreateOnly]
        public HueMotionSensorConfig config { get; set; }
        [DataMember, ExpandableObject, Category("State"), Description("State of the sensor"), CreateOnly]
        public PresenceSensorState state { get; set; }
        public string productid { get; set; }
        public string swconfigid { get; set; }
    }
}
