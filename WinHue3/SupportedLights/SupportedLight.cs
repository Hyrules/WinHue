using System;
using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace WinHue3
{

    public class Supportedlight
    {
        public string modelid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public Dictionary<string,ImageSource> img { get; set; }
        public MaxMin<ushort> hue { get; set; }
        public MaxMin<byte> bri { get; set; }
        public MaxMin<byte> sat { get; set; }
        public MaxMin<ushort> ct { get; set; }
    }

    public class MaxMin<T>
    {
        public T min { get; set; }
        public T max { get; set; }
    }

    public class SupportedLightConverter : JsonConverter
    {
        private readonly Type[] _types;

        public SupportedLightConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            
            JObject obj = (JObject)serializer.Deserialize(reader);

            Supportedlight supportedlight = new Supportedlight();

  
            if (obj["modelid"] != null)
                supportedlight.modelid = obj["modelid"].Value<string>();
            if (obj["name"] != null)
                supportedlight.name = obj["name"].Value<string>();
            if (obj["type"] != null)
                supportedlight.type = obj["type"].Value<string>();

            if (obj["hue"] != null)
            {
                if (obj["hue"]["max"] != null && obj["hue"]["min"] != null)
                    supportedlight.hue = new MaxMin<ushort>()
                    {
                        max = obj["hue"]["max"].Value<ushort>(),
                        min = obj["hue"]["min"].Value<ushort>()
                    };
            }

            if (obj["sat"] != null)
            {
                if (obj["sat"]["max"] != null && obj["sat"]["min"] != null)
                    supportedlight.sat = new MaxMin<byte>()
                    {
                        max = obj["sat"]["max"].Value<byte>(),
                        min = obj["sat"]["min"].Value<byte>()
                    };
            }

            if (obj["bri"] != null)
            {
                if (obj["bri"]["max"] != null && obj["bri"]["min"] != null)
                    supportedlight.bri = new MaxMin<byte>()
                    {
                        max = obj["bri"]["max"].Value<byte>(),
                        min = obj["bri"]["min"].Value<byte>()
                    };
            }

            if (obj["ct"] != null)
            {
                if (obj["ct"]["max"] != null && obj["ct"]["min"] != null)
                    supportedlight.ct = new MaxMin<ushort>()
                    {
                        max = obj["hue"]["max"].Value<ushort>(),
                        min = obj["hue"]["min"].Value<ushort>()
                    };
            }

            if (obj["img"] != null)
            {
                if (obj["img"]["on"] != null && obj["img"]["off"] != null && obj["img"]["unr"] != null)
                {
                    string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                    supportedlight.img = new Dictionary<string, ImageSource>();

                    if (File.Exists(path + @"\lights\images\" + obj["img"]["on"].Value<string>()) &&
                        File.Exists(path + @"\lights\images\" + obj["img"]["off"].Value<string>()) &&
                        File.Exists(path + @"\lights\images\" + obj["img"]["unr"].Value<string>()))
                    {
                        supportedlight.img.Add("on",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["on"].Value<string>())));
                        supportedlight.img.Add("off",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["off"].Value<string>())));
                        supportedlight.img.Add("unr",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["unr"].Value<string>())));
                    }
                    else
                    {
                        supportedlight.img.Add("on",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_on));
                        supportedlight.img.Add("off",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_off));
                        supportedlight.img.Add("unr",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_unr));
                    }
                }


            }
                
            return supportedlight;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Supportedlight) ;
        }
    }

}