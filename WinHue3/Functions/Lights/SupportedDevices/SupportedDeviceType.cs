using System.Collections.Generic;

namespace WinHue3.Functions.Lights.SupportedDevices
{
    public static class SupportedDeviceType
    {

        private static Dictionary<string,LightDevice> _deviceType;

        static SupportedDeviceType()
        {
            DeviceType = new Dictionary<string, LightDevice>();

            DeviceType.Add("Extended color light", new LightDevice()
            {
                Canbri = true,
                Canct = true,
                Canhue = true,
                Cansat = true,
                Canxy = true,
                Caneffect = true,
                Canalert = true
            });

            DeviceType.Add("Color light", new LightDevice()
            {
                Canbri = true,
                Canct = false,
                Canhue = true,
                Cansat = true,
                Canxy = true,
                Caneffect = true,
                Canalert = true
            });

            DeviceType.Add("Dimmable light", new LightDevice()
            {
                Canbri = true,
                Canct = false,
                Canhue = false,
                Cansat = false,
                Canxy = false,
                Caneffect = false,
                Canalert  = true
            });


            DeviceType.Add("Color temperature light", new LightDevice()
            {
                Canbri = true,
                Canct = true,
                Canhue = false,
                Cansat = false,
                Canxy = false,
                Canalert = true,
                Caneffect = false
            });

            DeviceType.Add("On/Off plug-in unit", new LightDevice()
            {
                Canbri = false,
                Canct = false,
                Canhue = false,
                Cansat = false,
                Canxy = false,
                Canalert = true,
                Caneffect = false

            });
        }

        public static Dictionary<string, LightDevice> DeviceType
        {
            get => _deviceType;

            internal set => _deviceType = value;
        }
    }



}
