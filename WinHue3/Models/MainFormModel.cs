using WinHue3.ViewModels;

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
            get => _sliderBri;
            set => SetProperty(ref _sliderBri,value);
        }

        public ushort SliderHue
        {
            get => _sliderHue;
            set => SetProperty(ref _sliderHue,value);
        }

        public ushort SliderCt
        {
            get => _sliderCT;
            set => SetProperty(ref _sliderCT,value);
        }

        public byte SliderSat
        {
            get => _sliderSat;
            set => SetProperty(ref _sliderSat,value);
        }

        public decimal SliderX
        {
            get => _sliderX;
            set => SetProperty(ref _sliderX,value);
        }

        public decimal SliderY
        {
            get => _sliderY;
            set => SetProperty(ref _sliderY,value);
        }

        public WinHueSortOrder Sort
        {
            get => _sort;
            set => SetProperty(ref _sort,value);
        }

        public byte OldSliderBri
        {
            get => _oldsliderBri;
            set => SetProperty(ref _oldsliderBri,value);
        }

        public ushort OldSliderHue
        {
            get => _oldsliderHue;
            set => SetProperty(ref _oldsliderHue,value);
        }

        public ushort OldSliderCt
        {
            get => _oldsliderCT;
            set => SetProperty(ref _oldsliderCT,value);
        }

        public byte OldSliderSat
        {
            get => _oldsliderSat;
            set => SetProperty(ref _oldsliderSat,value);
        }

        public decimal OldSliderX
        {
            get => _oldsliderX;
            set => SetProperty(ref _oldsliderX,value);
        }

        public decimal OldSliderY
        {
            get => _oldsliderY;
            set => SetProperty(ref _oldsliderY,value);
        }

        public bool ShowId
        {
            get => _showId;
            set => SetProperty(ref _showId,value);
        }
        
        public bool WrapText
        {
            get => _wrapText;
            set => SetProperty(ref _wrapText,value);
        }
    }
}
