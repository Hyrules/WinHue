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
            get => _canhue;

            set => _canhue = value;
        }

        public bool Canbri
        {
            get => _canbri;

            set => _canbri = value;
        }

        public bool Cansat
        {
            get => _cansat;

            set => _cansat = value;
        }

        public bool Canxy
        {
            get => _canxy;

            set => _canxy = value;
        }

        public bool Canct
        {
            get => _canct;

            set => _canct = value;
        }

        public bool Caneffect
        {
            get => _caneffect;

            set => _caneffect = value;
        }

        public bool Canalert
        {
            get => _canalert;

            set => _canalert = value;
        }
    }
}
