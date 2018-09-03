using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;


namespace WinHue3.Functions.RoomMap
{
   
    public class HueElement : ValidatableBindableBase
    {
        private double _x;
        private double _y;
        private double _panelHeight;
        private double _panelWidth;
        private double _imageHeight;
        private double _imageWidth;
        private Visibility _labelVisible;
        private string _label;
        private string _id;
        private ImageSource _image;

        public HueElement()
        {
            PanelHeight = 94;
            PanelWidth = 64;
            ImageHeight = 64;
            ImageWidth = 64;
        }

        public HueElement(double paneheight, double panelwidth, double imageheight, double imagewidth, string label, string id, ImageSource image = null)
        {
            PanelHeight = paneheight;
            PanelWidth = panelwidth;
            ImageHeight = imageheight;
            ImageWidth = imagewidth;
            Label = label;
            Id = id;
            Image = image;
        }

        public HueElement(Light l, double panelheight, double panelwidth, double imageheight, double imagewidth) : this(l)
        {
            PanelHeight = panelheight;
            PanelWidth = panelwidth;
            ImageHeight = ImageHeight;
            ImageWidth = ImageWidth;
        }

        public HueElement(Light l) : this()
        {
            Label = l.name;
            Id = l.Id;
            Image = l.Image;
        }

        public double PanelHeight
        {
            get => _panelHeight;
            set => SetProperty(ref _panelHeight,value);
        }

        public double PanelWidth
        {
            get => _panelWidth;
            set => SetProperty(ref _panelWidth,value);
        }

        public double ImageHeight
        {
            get => _imageHeight;
            set => SetProperty(ref _imageHeight,value);
        }

        public double ImageWidth
        {
            get => _imageWidth;
            set => SetProperty(ref _imageWidth,value);
        }

        public double X
        {
            get => _x;
            set => SetProperty(ref _x,value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y,value);
        }

        public Visibility LabelVisible
        {
            get => _labelVisible;
            set => SetProperty(ref _labelVisible,value);
        }

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        [JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }
    }

}
