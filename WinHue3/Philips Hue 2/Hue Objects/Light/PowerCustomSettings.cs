using System.ComponentModel;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    public class PowerCustomSettings : ValidatableBindableBase
    {
        private byte? _bri;
        private decimal[] _xy;
        private ushort? _ct;

        [DefaultValue(null)]
        public byte? bri
        {
            get => _bri;
            set => SetProperty(ref _bri, value);
        }

        [Editor(typeof(XYEditor), typeof(XYEditor)), DefaultValue(null)]
        public decimal[] xy
        {
            get => _xy;
            set => SetProperty(ref _xy ,value);
        }

        [DefaultValue(null)]
        public ushort? ct
        {
            get => _ct;
            set => SetProperty(ref _ct,value);
        }


    }
}
