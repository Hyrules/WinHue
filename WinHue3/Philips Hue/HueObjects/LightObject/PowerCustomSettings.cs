using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Annotations;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
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
