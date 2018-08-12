using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WinHue3.Philips_Hue.HueObjects.LightObject;


namespace WinHue3.Functions.RoomMap
{
    public class HueElement : Light
    {
        private double _x;
        private double _y;

        public double Height { get; set; }
        public double Width { get; set; }

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
    }
}
