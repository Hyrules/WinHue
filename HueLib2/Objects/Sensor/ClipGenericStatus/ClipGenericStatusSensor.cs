using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HueLib2.Objects.Interfaces;
using Newtonsoft.Json;

namespace HueLib2.Objects.Sensor.ClipGenericStatus
{
    [HueType("sensors")]
    public class ClipGenericStatusSensor : ISensor
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
        public ClipGenericStatusSensorConfig config { get; set; }
        public ClipGenericStatusState state { get; set; }
        public string productid { get; set; }
        public string swconfigid { get; set; }
    }
}
