using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.Animations
{
    public class AnimationCreatorViewModel : ValidatableBindableBase
    {
        private IHueObject _stepObject;
        private string _name;
        private string _description;
        private object _stepObjectProperty;

        public AnimationCreatorViewModel()
        {
        }

        public IHueObject StepObject { get => _stepObject; set => SetProperty(ref _stepObject,value); }
        public string Name { get => _name; set => SetProperty(ref _name,value); }
        public string Description { get => _description; set => SetProperty(ref _description,value); }
        public object StepObjectProperty { get => _stepObjectProperty; set => SetProperty(ref _stepObjectProperty,value); }
    }
}
