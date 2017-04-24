using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using HueLib2;
using WinHue3.Validation;

namespace WinHue3.Models
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
            _btnfourbg = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            _btnonebg = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            _btnthreebg = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            _btntwobg = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            _id = string.Empty;
            _buttonid = string.Empty;
        }

        public List<Scene> ListScenes
        {
            get{return _listScenes; }
            set { SetProperty(ref _listScenes, value); }
        }

        public SolidColorBrush BtnOneBG
        {
            get{return _btnonebg;}
            set { SetProperty(ref _btnonebg, value); }
        }

        public SolidColorBrush BtnTwoBG
        {
            get { return _btntwobg; }
            set { SetProperty(ref _btntwobg, value); }
        }

        public SolidColorBrush BtnThreeBG
        {
            get { return _btnthreebg; }
            set { SetProperty(ref _btnthreebg, value); }
        }

        public SolidColorBrush BtnFourBG
        {
            get { return _btnfourbg; }
            set { SetProperty(ref _btnfourbg, value); }
        }

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id ,value); }
        }

        [RequireHueTapButtonValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "HueTap_NoButtonSelected")]
        public string Buttonid
        {
            get { return _buttonid; }
            set { SetProperty(ref _buttonid,value); RaisePropertyChanged("CanSave"); }
        }
    }
}