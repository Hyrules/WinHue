using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.SupportedLights
{
    public static class SupportedDeviceType
    {

        private static Dictionary<string,LightDevice> _deviceType;

        static SupportedDeviceType()
        {
            DeviceType = new Dictionary<string, LightDevice>();

            DeviceType.Add("Extended Color Light", new LightDevice()
            {
                Canbri = true,
                Canct = true,
                Canhue = true,
                Cansat = true,
                Canxy = true,
                Caneffect = true,
                Canalert = true
            });

            DeviceType.Add("Color Light", new LightDevice()
            {
                Canbri = true,
                Canct = false,
                Canhue = true,
                Cansat = true,
                Canxy = true,
                Caneffect = true,
                Canalert = true
            });

            DeviceType.Add("Dimmable Light", new LightDevice()
            {
                Canbri = true,
                Canct = false,
                Canhue = false,
                Cansat = false,
                Canxy = false,
                Caneffect = false,
                Canalert  = true
            });


            DeviceType.Add("Color Temperature Light", new LightDevice()
            {
                Canbri = false,
                Canct = true,
                Canhue = false,
                Cansat = false,
                Canxy = false,
                Canalert = true,
                Caneffect = false
            });
        }

        public static Dictionary<string, LightDevice> DeviceType
        {
            get
            {
                return _deviceType;
            }

            internal set
            {
                _deviceType = value;
            }
        }
    }



}
