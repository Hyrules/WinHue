using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;

namespace WinHue3.Functions.Animations
{
    public class WaitAnimationAction: ValidatableBindableBase, IAnimationStep
    {
        private int _duration;
        private string _name;
        private string _description;
        private ImageSource _image;

        [Description("ms (1000 ms = 1 sec)")]
        public int Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string Name
        {
            get  => _name; 
            set => SetProperty(ref _name, value); 
        }

        public string Description
        {
            get => _description; 
            set => SetProperty(ref _description,value);
        }

        public ImageSource Image { get => _image; set => SetProperty(ref _image,value); }
    }
}
