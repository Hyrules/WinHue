using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.Animations
{
    public class AnimationCreatorViewModel : ValidatableBindableBase
    {

        private string _name;
        private string _description;
        private object _stepObjectProperty;
        private string _action;
        private List<IHueObject> _listAvailableHueObjects;
        private IHueObject _selectedHueObject;
        private ObservableCollection<IAnimationStep> _listSteps;

        public AnimationCreatorViewModel()
        {
            ListSteps = new ObservableCollection<IAnimationStep>();
        }

        public void Initialize(List<IHueObject> listHueObjects)
        {
            ListAvailableHueObjects = listHueObjects;
        }


        public string Name { get => _name; set => SetProperty(ref _name,value); }
        public string Description { get => _description; set => SetProperty(ref _description,value); }
        public string Action{ get => _action; set => SetProperty(ref _action, value);}
        public object StepObjectProperty { get => _stepObjectProperty; set => SetProperty(ref _stepObjectProperty,value); }

        public ICommand ChangeActionTargetCommand => new RelayCommand(param => ChangeActionTarget());
        public ICommand AddStepCommand => new RelayCommand(param => AddStep());
        public ICommand SelectHueObjectCommand => new RelayCommand(param => SelectHueObject());

        private void SelectHueObject()
        {

            StepObjectProperty = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
        }

        private void AddStep()
        {
            switch(_action)
            {
                case "Wait":
                    WaitAnimationAction wa = StepObjectProperty as WaitAnimationAction;
                    ListSteps.Add(wa);
                    break;
                case "Set a light or group":
                    SetObjectAnimationAction so = new SetObjectAnimationAction();
                    so.Objectype = SelectedHueObject.GetType();
                    so.Id = SelectedHueObject.Id;
                   //so.Body = Serializer.SerializeToJson(StepObjectProperty);
                    ListSteps.Add(so);
                    break;
                default:
                    break;
            }
        }

        public List<IHueObject> ListAvailableHueObjects
        {
            get => _listAvailableHueObjects;
            set => SetProperty(ref _listAvailableHueObjects,value); 
        }

        public IHueObject SelectedHueObject
        {
            get { return _selectedHueObject; }
            set { SetProperty(ref _selectedHueObject,value); }
        }

        public ObservableCollection<IAnimationStep> ListSteps
        {
            get { return _listSteps; }
            set { SetProperty(ref _listSteps,value); }
        }

        private void ChangeActionTarget()
        {
            switch (_action)
            {
                case "Wait":
                    StepObjectProperty = new WaitAnimationAction();
                    break;
                case "Set a light or group":
                    
                    break;
                default:
                    break;
                    
            }
        }
    }
}
