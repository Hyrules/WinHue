using System;
using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Mvvm;

namespace WinHue3
{

    public class Supportedlight : BindableBase
    {
        private string _modelid;
        private string _name;
        private string _type;
        private Dictionary<string, ImageSource> _img;
        private MaxMin<ushort> _hue;
        private MaxMin<byte> _bri;
        private MaxMin<byte> _sat;
        private MaxMin<ushort> _ct;

        public string Modelid
        {
            get
            {
                return _modelid;
            }

            set
            {
                SetProperty(ref _modelid,value);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                SetProperty(ref _type,value);
            }
        }

        public Dictionary<string, ImageSource> Img
        {
            get
            {
                return _img;
            }

            set
            {
                SetProperty(ref _img,value);
            }
        }

        public MaxMin<ushort> Hue
        {
            get
            {
                return _hue;
            }

            set
            {
                SetProperty(ref _hue,value);
            }
        }

        public MaxMin<byte> Bri
        {
            get
            {
                return _bri;
            }

            set
            {
                SetProperty(ref _bri,value);
            }
        }

        public MaxMin<byte> Sat
        {
            get
            {
                return _sat;
            }

            set
            {
                SetProperty(ref _sat,value);
            }
        }

        public MaxMin<ushort> Ct
        {
            get
            {
                return _ct;
            }

            set
            {
                SetProperty(ref _ct,value);
            }
        }
    }

    public class MaxMin<T> : BindableBase
    {
        private T _min;
        private T _max;

        public MaxMin(T maxval,T minval)
        {
           _min = minval;
           _max = maxval;
        }

        public MaxMin()
        {
        }

        public T Min
        {
            get { return _min; }
            set { SetProperty(ref _min, value); }
        }
        public T Max
        {
            get { return _max; }
            set { SetProperty(ref _max, value); }
        }

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
                supportedlight.Modelid = obj["modelid"].Value<string>();
            if (obj["name"] != null)
                supportedlight.Name = obj["name"].Value<string>();
            if (obj["type"] != null)
                supportedlight.Type = obj["type"].Value<string>();

            if (obj["hue"] != null)
            {
                if (obj["hue"]["max"] != null && obj["hue"]["min"] != null)
                    supportedlight.Hue = new MaxMin<ushort>(obj["hue"]["max"].Value<ushort>(), obj["hue"]["min"].Value<ushort>());
            }

            if (obj["sat"] != null)
            {
                if (obj["sat"]["max"] != null && obj["sat"]["min"] != null)
                    supportedlight.Sat = new MaxMin<byte>(obj["sat"]["max"].Value<byte>(), obj["sat"]["min"].Value<byte>());
            }

            if (obj["bri"] != null)
            {
                if (obj["bri"]["max"] != null && obj["bri"]["min"] != null)
                    supportedlight.Bri = new MaxMin<byte>(obj["bri"]["max"].Value<byte>(), obj["bri"]["min"].Value<byte>());

            }

            if (obj["ct"] != null)
            {
                if (obj["ct"]["max"] != null && obj["ct"]["min"] != null)
                    supportedlight.Ct = new MaxMin<ushort>(obj["ct"]["max"].Value<ushort>(), obj["ct"]["min"].Value<ushort>());
            }

            if (obj["img"] != null)
            {
                if (obj["img"]["on"] != null && obj["img"]["off"] != null && obj["img"]["unr"] != null)
                {
                    string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                    supportedlight.Img = new Dictionary<string, ImageSource>();

                    if (File.Exists(path + @"\lights\images\" + obj["img"]["on"].Value<string>()) &&
                        File.Exists(path + @"\lights\images\" + obj["img"]["off"].Value<string>()) &&
                        File.Exists(path + @"\lights\images\" + obj["img"]["unr"].Value<string>()))
                    {
                        supportedlight.Img.Add("on",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["on"].Value<string>())));
                        supportedlight.Img.Add("off",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["off"].Value<string>())));
                        supportedlight.Img.Add("unr",
                            new BitmapImage(new Uri(path + @"\lights\images\" + obj["img"]["unr"].Value<string>())));
                    }
                    else
                    {
                        supportedlight.Img.Add("on",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_on));
                        supportedlight.Img.Add("off",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_off));
                        supportedlight.Img.Add("unr",GDIManager.CreateImageSourceFromImage(Properties.Resources.DefaultLight_unr));
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