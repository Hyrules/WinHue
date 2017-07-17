using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WinHue3.Models
{
    public enum WinHueSortOrder
    {
        Default,
        Ascending,
        Descending
    }

    public class MainFormModel : ValidatableBindableBase
    {


        private byte _sliderBri;
        private ushort _sliderHue;
        private ushort _sliderCT;
        private byte _sliderSat;
        private decimal _sliderX;
        private decimal _sliderY;

        private byte _oldsliderBri;
        private ushort _oldsliderHue;
        private ushort _oldsliderCT;
        private byte _oldsliderSat;
        private decimal _oldsliderX;
        private decimal _oldsliderY;
        private bool _showId;
        private bool _wrapText;

        private WinHueSortOrder _sort;

        public MainFormModel()
        {
            _sort = WinHueSortOrder.Default;
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

        public WinHueSortOrder Sort
        {
            get { return _sort; }
            set { SetProperty(ref _sort,value); }
        }

        public byte OldSliderBri
        {
            get { return _oldsliderBri; }
            set { SetProperty(ref _oldsliderBri,value); }
        }

        public ushort OldSliderHue
        {
            get { return _oldsliderHue; }
            set { SetProperty(ref _oldsliderHue,value); }
        }

        public ushort OldSliderCt
        {
            get { return _oldsliderCT; }
            set { SetProperty(ref _oldsliderCT,value); }
        }

        public byte OldSliderSat
        {
            get { return _oldsliderSat; }
            set { SetProperty(ref _oldsliderSat,value); }
        }

        public decimal OldSliderX
        {
            get { return _oldsliderX; }
            set { SetProperty(ref _oldsliderX,value); }
        }

        public decimal OldSliderY
        {
            get { return _oldsliderY; }
            set { SetProperty(ref _oldsliderY,value); }
        }

        public bool ShowId
        {
            get { return _showId; }
            set { SetProperty(ref _showId,value); }
        }
        
        public bool WrapText
        {
            get { return _wrapText;}
            set {SetProperty(ref _wrapText,value); }
        }
    }
}
