using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.SupportedLights
{
    public class LightDevice
    {
        bool _canhue;
        bool _canbri;
        bool _cansat;
        bool _canxy;
        bool _canct;
        bool _caneffect;
        bool _canalert;

        public LightDevice()
        {
            _canalert = false;
            _canbri = false;
            _canct = false;
            _caneffect = false;
            _canhue = false;
            _cansat = false;
            _canxy = false;
        }

        public bool Canhue
        {
            get
            {
                return _canhue;
            }

            set
            {
                _canhue = value;
            }
        }

        public bool Canbri
        {
            get
            {
                return _canbri;
            }

            set
            {
                _canbri = value;
            }
        }

        public bool Cansat
        {
            get
            {
                return _cansat;
            }

            set
            {
                _cansat = value;
            }
        }

        public bool Canxy
        {
            get
            {
                return _canxy;
            }

            set
            {
                _canxy = value;
            }
        }

        public bool Canct
        {
            get
            {
                return _canct;
            }

            set
            {
                _canct = value;
            }
        }

        public bool Caneffect
        {
            get
            {
                return _caneffect;
            }

            set
            {
                _caneffect = value;
            }
        }

        public bool Canalert
        {
            get
            {
                return _canalert;
            }

            set
            {
                _canalert = value;
            }
        }
    }
}
