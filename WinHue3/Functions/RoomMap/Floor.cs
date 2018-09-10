using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;

namespace WinHue3.Functions.RoomMap
{
    [JsonConverter(typeof(FloorPlanConverter))]
    public class Floor
    {
        public Floor()
        {
            Name = string.Empty;
            Image = null;
            Elements = new List<HueElement>();
            CanvasHeight = 600;
            CanvasWidth = 800;
        }

        public string Name { get; set; }
        public BitmapImage Image { get; set; }
        public List<HueElement> Elements { get; set; }
        public double CanvasHeight { get; set; }
        public double CanvasWidth { get; set; }
        public Stretch StretchMode { get; set; }
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
            writer.WriteValue(floorplan.Image.ToBase64());
            writer.WritePropertyName("CanvasHeight");
            writer.WriteValue(floorplan.CanvasHeight);
            writer.WritePropertyName("CanvasWidth");
            writer.WriteValue(floorplan.CanvasWidth);
            writer.WritePropertyName("StretchMode");
            writer.WriteValue(floorplan.StretchMode);
            writer.WritePropertyName("Elements");
            
            writer.WriteStartArray();
            foreach (HueElement h in floorplan.Elements)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("PanelHeight");
                writer.WriteValue(h.PanelHeight);
                writer.WritePropertyName("PanelWidth");
                writer.WriteValue(h.PanelWidth);
                writer.WritePropertyName("ImageHeight");
                writer.WriteValue(h.ImageHeight);
                writer.WritePropertyName("ImageWidth");
                writer.WriteValue(h.ImageWidth);
                writer.WritePropertyName("X");
                writer.WriteValue(h.X);
                writer.WritePropertyName("Y");
                writer.WriteValue(h.Y);
                writer.WritePropertyName("Label");
                writer.WriteValue(h.Label);
                writer.WritePropertyName("ID");
                writer.WriteValue(h.Id);
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
            JArray elem = obj["Elements"].Value<JArray>();
            foreach (JObject job in elem)
            {
                floorplan.Elements.Add(job.ToObject<HueElement>());
            }
            string base64image = obj["Image"]?.Value<string>();
            floorplan.Image = Base64StringToBitmap(base64image);
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
