using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Annotations;
using WinHue3.ExtensionMethods;
using WinHue3.Utils;

namespace WinHue3.Functions.RoomMap
{
    [JsonConverter(typeof(FloorPlanConverter))]
    public class Floor : INotifyPropertyChanged
    {
        private string _name;
        private BitmapImage _image;
        private ObservableCollection<HueElement> _elements;
        private double _canvasHeight;
        private double _canvasWidth;
        private Stretch _stretchMode;
        private bool _isChanged;

        public Floor()
        {
            Name = string.Empty;
            Image = null;
            Elements = new ObservableCollection<HueElement>();
            CanvasHeight = 0;
            CanvasWidth = 0;
            IsChanged = false;
            Elements.CollectionChanged += Elements_CollectionChanged;
        }

        private void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (object o in e.NewItems)
                    {
                        ((HueElement)o).ElementModified += Floor_ElementModified;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (object o in e.OldItems)
                    {
                        ((HueElement)o).ElementModified -= Floor_ElementModified;
                    }
                    
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
            }
            this.IsChanged = true;
            RaisePropertyChanged("IsModified");
        }

        private void Floor_ElementModified(object sender, EventArgs e)
        {
            IsChanged = true;
            RaisePropertyChanged("IsModified");
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public BitmapImage Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        public ObservableCollection<HueElement> Elements
        {
            get => _elements; 
            set => SetProperty(ref _elements,value);
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight,value);
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth,value);
        }

        public Stretch StretchMode
        {
            get => _stretchMode; 
            set => SetProperty(ref _stretchMode,value);
        }

        public bool IsChanged
        {
            get => _isChanged || Elements.Any(x => x.IsChanged); 
            private set => SetProperty(ref _isChanged ,value);
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            RaisePropertyChanged("IsChanged");
            return true;
        }

        public void AcceptChanges()
        {
            foreach (HueElement he in _elements)
            {
                he.AcceptChanges();
            }

            IsChanged = false;
            RaisePropertyChanged("IsChanged");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {

            if (propertyName != "IsChanged")
            {
                IsChanged = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }

    class FloorPlanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if (value == null)
            {
                serializer.Serialize(writer, null);
            }

            Floor floorplan = (Floor)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(floorplan.Name);
            writer.WritePropertyName("Image");
            writer.WriteValue(floorplan.Image?.ToBase64());
            writer.WritePropertyName("CanvasHeight");
            writer.WriteValue(floorplan.CanvasHeight);
            writer.WritePropertyName("CanvasWidth");
            writer.WriteValue(floorplan.CanvasWidth);
            writer.WritePropertyName("StretchMode");
            writer.WriteValue(floorplan.StretchMode);
            writer.WritePropertyName("Elements");
            writer.WriteStartArray();
            foreach (HueElement he in floorplan.Elements)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("PanelHeight");
                writer.WriteValue(he.PanelHeight);
                writer.WritePropertyName("PanelWidth");
                writer.WriteValue(he.PanelWidth);
                writer.WritePropertyName("ImageHeight");
                writer.WriteValue(he.ImageHeight);
                writer.WritePropertyName("ImageWidth");
                writer.WriteValue(he.ImageWidth);
                writer.WritePropertyName("X");
                writer.WriteValue(he.X);
                writer.WritePropertyName("Y");
                writer.WriteValue(he.Y);
                writer.WritePropertyName("Id");
                writer.WriteValue(he.Id);
                writer.WritePropertyName("HueType");
                writer.WriteValue(he.HueType);
                writer.WritePropertyName("Label");
                writer.WriteValue(he.Label);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Floor floorplan = new Floor();

            JObject obj = serializer.Deserialize<JObject>(reader);
            floorplan.Name = obj["Name"]?.Value<string>();
            JArray elem = obj["Elements"].ToObject<JArray>();
            foreach (JObject job in elem)
            {
                HueElement he = new HueElement();
                he.PanelHeight = job["PanelHeight"]?.Value<double>() ?? he.PanelHeight;
                he.PanelWidth = job["PanelWidth"]?.Value<double>() ?? he.PanelWidth;
                he.ImageHeight = job["ImageHeight"]?.Value<double>() ?? he.ImageHeight;
                he.ImageWidth = job["ImageWidth"]?.Value<double>() ?? he.ImageWidth;
                he.X = job["X"]?.Value<double>() ?? 0;
                he.Y = job["Y"]?.Value<double>() ?? 0;
                he.Id = job["Id"]?.Value<string>();
                he.HueType = (HueElementType)Enum.Parse(typeof(HueElementType),job["HueType"]?.Value<string>() ?? "0");
                he.Label = job["Label"]?.Value<string>();
                floorplan.Elements.Add(he);
            }
            string base64image = obj["Image"]?.Value<string>();
            if (base64image != null)
            {
                floorplan.Image = Base64StringToBitmap(base64image);
            }

            floorplan.CanvasWidth = obj["CanvasWidth"]?.Value<double>() ?? 800;
            floorplan.CanvasHeight = obj["CanvasHeight"]?.Value<double>() ?? 600;
            int? sm = obj["StretchMode"]?.Value<int>();
            if (sm != null)
            {
                floorplan.StretchMode = (Stretch) Enum.ToObject(typeof(Stretch), sm);
            }
            else
            {
                floorplan.StretchMode = Stretch.None;
            }

            floorplan.AcceptChanges();

            return floorplan;
        }

        private static BitmapImage Base64StringToBitmap(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            BitmapImage bi = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(byteBuffer))
            {
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.None;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
            }
            return bi;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Floor);
        }
    }
}
