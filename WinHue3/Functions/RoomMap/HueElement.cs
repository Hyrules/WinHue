using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinHue3.Philips_Hue.HueObjects.LightObject;


namespace WinHue3.Functions.RoomMap
{
    public class HueElement : Light
    {
        private double _x;
        private double _y;
        private double _panelHeight;
        private double _panelWidth;
        private double _imageHeight;
        private double _imageWidth;
        private Visibility _labelVisible;
        private string _label;

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

        public HueElement(Light l)
        {
            name = l.name;
            state = l.state;
            Id = l.Id;
            luminaireuniqueid = l.luminaireuniqueid;
            uniqueid = l.uniqueid;
            manufacturername = l.manufacturername;
            modelid = l.modelid;
            swversion = l.swversion;
            type = l.type;
            PanelHeight = 94;
            PanelWidth = 64;
            ImageHeight = 64;
            ImageWidth = 64;
            Label = name;
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
            set
            {
                SetProperty(ref _label,value);
            } 
        }
    }
}
