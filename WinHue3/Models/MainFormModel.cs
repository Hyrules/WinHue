using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models
{
    public class MainFormModel : ValidatableBindableBase
    {
        private byte _sliderBri;
        private ushort _sliderHue;
        private ushort _sliderCT;
        private byte _sliderSat;
        private decimal _sliderX;
        private decimal _sliderY;
        private uint _sliderTT;

        public MainFormModel()
        {
                      
        }

        public byte SliderBri
        {
            get { return _sliderBri; }
            set { SetProperty(ref _sliderBri,value); }
        }

        public ushort SliderHue
        {
            get { return _sliderHue; }
            set { SetProperty(ref _sliderHue,value); }
        }

        public ushort SliderCt
        {
            get { return _sliderCT; }
            set { SetProperty(ref _sliderCT,value); }
        }

        public byte SliderSat
        {
            get { return _sliderSat; }
            set { SetProperty(ref _sliderSat,value); }
        }

        public decimal SliderX
        {
            get { return _sliderX; }
            set { SetProperty(ref _sliderX,value); }
        }

        public decimal SliderY
        {
            get { return _sliderY; }
            set { SetProperty(ref _sliderY,value); }
        }

        public uint SliderTt
        {
            get { return _sliderTT; }
            set { SetProperty(ref _sliderTT,value); }
        }
    }
}
