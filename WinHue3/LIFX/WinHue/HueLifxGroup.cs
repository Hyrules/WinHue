using System.Windows.Media;
using WinHue3.Utils;

namespace WinHue3.LIFX.WinHue
{
    public class HueLifxGroup : ValidatableBindableBase, ILifxObject
    {
        public HueLifxGroup()
        {
            
        }

        private string _name;
        private ImageSource _image;

        public string Name {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

    }
}
