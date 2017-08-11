using System;

namespace WinHue3.Philips_Hue.HueObjects.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class HueType : Attribute
    {
        private string type;
        public HueType(string type)
        {
            this.type = type;
        }

        public string HueObjectType
        {
            get => type;
            set => type = value;
        }
    }
}
