using System;
using System.Threading.Tasks;
using System.Windows.Media;
using WinHue3.Functions.Lights.Finder;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.LIFX.Framework;
using WinHue3.LIFX.Framework.Responses;
using WinHue3.LIFX.Framework.Responses.States.Device;
using WinHue3.LIFX.Framework.Responses.States.Light;
using WinHue3.Utils;

namespace WinHue3.LIFX.WinHue
{
    public class HueLifxLight : ValidatableBindableBase, ILifxObject
    {
        private ImageSource _image;
        private LifxLight _light;
        private LifxProduct _product;

        private HueLifxLight(LifxLight light)
        {
            Light = light;   
        }

        public static async Task<HueLifxLight> CreateLightAsync(LifxLight light)
        {
            HueLifxLight newlight = new HueLifxLight(light);
            LifxCommMessage<StateVersion> version = await light.GetVersionAsync();
            if (!version.Error)
            {
                newlight._product = LifxVendors.GetProduct(BitConverter.ToInt32(version.Data.Vendor, 0),
                    BitConverter.ToInt32(version.Data.Product, 0));

            }
 
            newlight.Light = await LifxLight.CreateLightAsync(light.IP, light.Mac);
            newlight.Image = LightImageLibrary.Images.ContainsKey(newlight.Version) ? LightImageLibrary.Images[newlight.Version][newlight.On ? "on" : "off"] : LightImageLibrary.Images["DefaultLIFX"][newlight.On ? "on" : "off"];
            return newlight;
        }

        public string Name =>Light.Label;
        public string Version => _product.name;
        public LifxFeatures Features => _product.features;
        public bool On => _light.Power != 0;
        public ImageSource Image { get => _image; set => SetProperty(ref _image,value); }

        public LifxLight Light
        {
            get { return _light; }
            set { SetProperty(ref _light,value); }
        }
    }
}
