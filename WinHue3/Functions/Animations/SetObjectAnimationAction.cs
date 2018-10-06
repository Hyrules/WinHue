using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.Animations
{
    public class SetObjectAnimationAction : ValidatableBindableBase, IAnimationStep
    {
        private Type _objectype;
        private string _id;
        private string _body;
        private string _name;
        private string _description;
        private ImageSource _image;

        public Type Objectype
        {
            get => _objectype;
            set => SetProperty(ref _objectype,value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Body
        {
            get => _body;
            set => SetProperty(ref _body,value);
        }

        public ImageSource Image { get => _image;
            set => SetProperty(ref _image, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }
    }
}
