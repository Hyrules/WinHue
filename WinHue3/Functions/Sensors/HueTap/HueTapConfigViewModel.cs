using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;
using WinHue3.Validations;

namespace WinHue3.Functions.Sensors.HueTap
{
    public class HueTapConfigViewModel : ValidatableBindableBase
    {
        private readonly System.Windows.Media.Color _selectedColor = System.Windows.Media.Color.FromArgb(128, 255, 0, 0);
        private readonly System.Windows.Media.Color _deselectedColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
        private Scene _selectedScene;

        private HueTapModel _huetapmodel;

        public HueTapConfigViewModel()
        {
            _huetapmodel = new HueTapModel();
        }

        public bool CanSave => HueTapModel.Buttonid != string.Empty && SelectedScene != null;

        private void BtnOne()
        {
            HueTapModel.BtnOneBG = new SolidColorBrush(_selectedColor);
            HueTapModel.BtnTwoBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnThreeBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnFourBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.Buttonid = "34";
        }

        private void BtnTwo()
        {
            HueTapModel.BtnOneBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnTwoBG = new SolidColorBrush(_selectedColor);
            HueTapModel.BtnThreeBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnFourBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.Buttonid = "16";
        }

        private void BtnThree()
        {
            HueTapModel.BtnOneBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnThreeBG = new SolidColorBrush(_selectedColor);
            HueTapModel.BtnTwoBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnFourBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.Buttonid = "18";

        }

        private void BtnFour()
        {
            HueTapModel.BtnOneBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnFourBG = new SolidColorBrush(_selectedColor);
            HueTapModel.BtnTwoBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.BtnThreeBG = new SolidColorBrush(_deselectedColor);
            HueTapModel.Buttonid = "17";

        }

        private void BtnAssign()
        {

            Rule newRule = new Rule
            {
                name = $"TAP {HueTapModel.Buttonid}",
                actions = new RuleActionCollection()
                {
                    new RuleAction()
                    {
                        address = new HueAddress() {objecttype = "groups", id = "0", property = "action"},
                        body = $"{{\"scene\":\"{_selectedScene.Id}\"}}",
                        method = "PUT"
                    }
                },
                conditions = new List<RuleCondition>
                {
                    new RuleCondition()
                    {
                        address = new HueAddress() { objecttype = "sensors", id = HueTapModel.Id, property = "state", subprop = "buttonevent"},
                        @operator = "eq",
                        value = HueTapModel.Buttonid
                    },
                    new RuleCondition()
                    {
                        address = new HueAddress() {objecttype = "sensors", id = HueTapModel.Id, property = "state", subprop = "lastupdated"},
                        @operator = "dx"
                    }
                }
            };

            bool result = BridgeManager.BridgeManager.Instance.SelectedBridge.CreateObject(newRule);
            if (result)
            {
                HueTapModel.BtnOneBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnTwoBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnThreeBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnFourBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.Buttonid = string.Empty;
                SelectedScene = null;
                RaisePropertyChanged("CanSave");
            }
            else
            {
                BridgeManager.BridgeManager.Instance.SelectedBridge.ShowErrorMessages();
            }
    
        }
    

        public ICommand BtnOneCommand => new RelayCommand(param => BtnOne());
        public ICommand BtnTwoCommand => new RelayCommand(param => BtnTwo());
        public ICommand BtnThreeCommand => new RelayCommand(param => BtnThree());
        public ICommand BtnFourCommand => new RelayCommand(param => BtnFour());
        public ICommand BtnAssignCommand => new RelayCommand(param => BtnAssign());

        public HueTapModel HueTapModel
        {
            get => _huetapmodel;
            set => SetProperty(ref _huetapmodel,value);
        }

        [NotNullValidation(ErrorMessageResourceType= typeof(GlobalStrings), ErrorMessageResourceName = "HueTap_SelectAScene")]     
        public Scene SelectedScene
        {
            get => _selectedScene;
            set { SetProperty(ref _selectedScene,value); RaisePropertyChanged("CanSave"); }
        }


    }
}