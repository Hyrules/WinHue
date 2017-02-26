using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HueLib2;
using HueLib2.Objects.Rules;
using WinHue3.Models;
using WinHue3.Validation;


namespace WinHue3.ViewModels
{
    public class HueTapConfigViewModel : ValidatableBindableBase
    {
        private readonly Color _selectedColor = Color.FromArgb(128, 255, 0, 0);
        private readonly Color _deselectedColor = Color.FromArgb(0, 0, 0, 0);
        private Scene _selectedScene;
        private Bridge _bridge;

        private HueTapModel _huetapmodel;

        public HueTapConfigViewModel()
        {
            HueTapModel = new HueTapModel();
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
                actions = new List<RuleAction>
                {
                    new RuleAction()
                    {
                        address = new RuleAddress() {objecttype = "groups", id = "0", property = "action"},
                        body = new SceneBody() {scene = SelectedScene.Id},
                        method = "PUT"
                    }
                },
                conditions = new List<RuleCondition>
                {
                    new RuleCondition()
                    {
                        address = new RuleAddress() { objecttype = "sensors", id = HueTapModel.Id, property = "state", subprop = "buttonevent"},
                        @operator = "eq",
                        value = HueTapModel.Buttonid
                    },
                    new RuleCondition()
                    {
                        address = new RuleAddress() {objecttype = "sensors", id = HueTapModel.Id, property = "state", subprop = "lastupdated"},
                        @operator = "dx"
                    }
                }
            };

            CommandResult comres = _bridge.CreateObject<Rule>(newRule);
            if (comres.Success)
            {
                HueTapModel.BtnOneBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnTwoBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnThreeBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.BtnFourBG = new SolidColorBrush(_deselectedColor);
                HueTapModel.Buttonid = string.Empty;
                SelectedScene = null;
                OnPropertyChanged("CanSave");
            }
            else
            {
                _bridge.ShowErrorMessages();
            }
    
        }
    

        public ICommand BtnOneCommand => new RelayCommand(param => BtnOne());
        public ICommand BtnTwoCommand => new RelayCommand(param => BtnTwo());
        public ICommand BtnThreeCommand => new RelayCommand(param => BtnThree());
        public ICommand BtnFourCommand => new RelayCommand(param => BtnFour());
        public ICommand BtnAssignCommand => new RelayCommand(param => BtnAssign());

        public HueTapModel HueTapModel
        {
            get { return _huetapmodel; }
            set { SetProperty(ref _huetapmodel,value); }
        }

        [NotNullValidation(ErrorMessageResourceType= typeof(GlobalStrings), ErrorMessageResourceName = "HueTap_SelectAScene")]     
        public Scene SelectedScene
        {
            get { return _selectedScene; }
            set { SetProperty(ref _selectedScene,value); OnPropertyChanged("CanSave"); }
        }

        public Bridge Bridge
        {
            get { return _bridge; }
            set { SetProperty(ref _bridge,value); }
        }
    }
}