using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Utils
{
    public class HueObject<T> : ValidatableBindableBase where T : IHueObject
    {
        private ImageSource _image;
        private string _id;
        private T _hueobject;
        private readonly Type _type;

        public HueObject()
        {
            _type = typeof(T);
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }

        public string Name => _hueobject.name;

        public T Hueobject
        {
            get => _hueobject;
            set => SetProperty( ref _hueobject,value);
        }

        public Type Type => _type;

    }
}
