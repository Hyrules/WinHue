using System.Collections.Generic;
using System.Windows.Media;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Sensors.HueTap
{
    public class HueTapModel : ValidatableBindableBase
    {
        private List<Scene> _listScenes;
        private SolidColorBrush _btnonebg;
        private SolidColorBrush _btntwobg;
        private SolidColorBrush _btnthreebg;
        private SolidColorBrush _btnfourbg;
        private string _id;
        private string _buttonid;

        public HueTapModel()
        {
            _listScenes = new List<Scene>();
            _btnfourbg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,0,0,0));
            _btnonebg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,0,0,0));
            _btnthreebg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,0,0,0));
            _btntwobg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,0,0,0));
            _id = string.Empty;
            _buttonid = string.Empty;
        }

        public List<Scene> ListScenes
        {
            get => _listScenes;
            set => SetProperty(ref _listScenes, value);
        }

        public SolidColorBrush BtnOneBG
        {
            get => _btnonebg;
            set => SetProperty(ref _btnonebg, value);
        }

        public SolidColorBrush BtnTwoBG
        {
            get => _btntwobg;
            set => SetProperty(ref _btntwobg, value);
        }

        public SolidColorBrush BtnThreeBG
        {
            get => _btnthreebg;
            set => SetProperty(ref _btnthreebg, value);
        }

        public SolidColorBrush BtnFourBG
        {
            get => _btnfourbg;
            set => SetProperty(ref _btnfourbg, value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id ,value);
        }

        [RequireHueTapButtonValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "HueTap_NoButtonSelected")]
        public string Buttonid
        {
            get => _buttonid;
            set { SetProperty(ref _buttonid,value); RaisePropertyChanged("CanSave"); }
        }
    }
}