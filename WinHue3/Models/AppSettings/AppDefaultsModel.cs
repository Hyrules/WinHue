using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models.AppSettings
{
    public class AppDefaultsModel : ValidatableBindableBase
    {
        private ushort? _allOnTT;
        private ushort? _allOffTT;
        private ushort? _defaultTT;
        private byte _defaultLightBri;
        private byte _defaultGroupBri;

        public AppDefaultsModel()
        {
            AllOffTt = null;
            AllOnTt = null;
            DefaultTt = null;
            DefaultLightBri = 255;
            DefaultGroupBri = 255;
        }

        public ushort? AllOnTt
        {
            get { return _allOnTT; }
            set { SetProperty(ref _allOnTT,value); }
        }

        public ushort? AllOffTt
        {
            get { return _allOffTT; }
            set { SetProperty(ref _allOffTT,value); }
        }

        public ushort? DefaultTt
        {
            get { return _defaultTT; }
            set { SetProperty(ref _defaultTT,value); }
        }

        public byte DefaultLightBri
        {
            get { return _defaultLightBri; }
            set { SetProperty(ref _defaultLightBri,value); }
        }

        public byte DefaultGroupBri
        {
            get { return _defaultGroupBri; }
            set { SetProperty(ref _defaultGroupBri,value); }
        }
    }
}
