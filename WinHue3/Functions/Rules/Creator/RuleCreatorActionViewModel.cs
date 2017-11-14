using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;

namespace WinHue3.Functions.Rules.Creator
{
    public class RuleCreatorActionViewModel : ValidatableBindableBase
    {
        private ObservableCollection<IHueObject> _listActionHueObjects;
        private ObservableCollection<RuleAction> _listRuleActions;
        private RuleAction _selectedRuleAction;

        private Bridge _bridge;
        private string _objectType;
        private IHueObject _selectedHueObject;
        private object _actionProperties;

        public RuleCreatorActionViewModel()
        {
            _listActionHueObjects = new ObservableCollection<IHueObject>();
            _listRuleActions = new ObservableCollection<RuleAction>();
        }

        public ICommand PopulateActionObjectsCommand => new RelayCommand(param => PopulateActionObjects());
        public ICommand PopulatePropertyGridCommand => new RelayCommand(param => PopulatePropertyGrid());
        public ICommand AddActionCommand => new RelayCommand(param => AddAction(), (param) => CanAddAction());
        public ICommand RemoveRuleActionCommand => new RelayCommand(param => RemoveRuleAction());
        public ICommand SelectRuleActionCommand => new RelayCommand(param => SelectRuleAction());

        private void SelectRuleAction()
        {
            if (SelectedRuleAction == null) return;
            if (SelectedRuleAction.address.objecttype == "groups" && SelectedRuleAction.address.id == "0")
            {
                ObjectType = "scenes";
                string id = Serializer.DeserializeToObject<SceneBody>(SelectedRuleAction.body).scene;
                SelectedHueObject =
                    ListActionHueObjects.FirstOrDefault(x => x.Id == id);
            }
            else
            {
                IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedRuleAction.address.objecttype);
                JsonConvert.PopulateObject(SelectedRuleAction.body,bp);
                ObjectType = SelectedRuleAction.address.objecttype;
                SelectedHueObject = ListActionHueObjects.FirstOrDefault(x => x.Id == SelectedRuleAction.address.id);
                ActionProperties = bp;
            }
        }

        private void RemoveRuleAction()
        {
            ListRuleActions.Remove(SelectedRuleAction);
            SelectedRuleAction = null;
        }

        private bool CanAddAction()
        {
            if (ListRuleActions.Count > 7) return false;
            if (SelectedHueObject == null) return false;
            if (ObjectType == null) return false;
            if (Serializer.SerializeToJson(ActionProperties) == "{}") return false;
            return true;
        }

        private void AddAction()
        {
            DialogResult result = DialogResult.Yes;

            RuleAction ra = new RuleAction();
            string address = string.Empty;
            switch (ObjectType)
            {
                case "lights":
                    address = $"/{ObjectType}/{SelectedHueObject.Id}/state";
                    break;
                case "groups":
                    address = $"/{ObjectType}/{SelectedHueObject.Id}/action";
                    break;
                case "schedules":
                    address = $"/{ObjectType}/{SelectedHueObject.Id}";
                    break;
                case "resourcelinks":
                    address = $"/{ObjectType}/{SelectedHueObject.Id}/";
                    break;
                case "scenes":
                    address = $"/groups/0/action";
                    break;
                case "sensors":
                    break;
                default:
                    break;
            }

            if (ListRuleActions.Any(x => x.address.ToString() == address))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ActionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleActions.Remove(ListRuleActions.FirstOrDefault(x => x.address.ToString() == address));
            }

            if (result != DialogResult.Yes) return;
            ra.address = new HueAddress(address);
            ra.method = "PUT";
            ra.body = _actionProperties != null ? Serializer.SerializeToJson(_actionProperties) : Serializer.SerializeToJson(new SceneBody() {scene = SelectedHueObject.Id});
            ListRuleActions.Add(ra);
        }

        private void PopulatePropertyGrid()
        {
            switch (ObjectType)
            {
                case "lights":
                    ActionProperties = new State();
                    break;
                case "groups":
                    ActionProperties = new Action();
                    break;
                case "schedules":
                    ActionProperties = JObject.Parse(((Schedule)_selectedHueObject).command.body);
                    break;
                case "scenes":
                    ActionProperties = null;
                    break;
                case "sensors":
                    ActionProperties = ((Sensor) _selectedHueObject).state;
                    break;
                default:
                    break;
            }   
                          
        }

        public ObservableCollection<IHueObject> ListActionHueObjects
        {
            get => _listActionHueObjects;
            set => SetProperty(ref _listActionHueObjects,value);
        }

        public ObservableCollection<RuleAction> ListRuleActions
        {
            get => _listRuleActions;
            set => SetProperty(ref _listRuleActions,value);
        }

        public string ObjectType
        {
            get => _objectType;
            set => SetProperty(ref _objectType,value);
        }

        public IHueObject SelectedHueObject
        {
            get => _selectedHueObject;
            set => SetProperty(ref _selectedHueObject,value);
        }

        public object ActionProperties
        {
            get => _actionProperties;
            set => SetProperty(ref _actionProperties,value);
        }

        public RuleAction SelectedRuleAction
        {
            get => _selectedRuleAction;
            set => SetProperty(ref _selectedRuleAction,value);
        }


        private void PopulateActionObjects()
        {
            ListActionHueObjects.Clear();
            ActionProperties = null;
            switch (ObjectType)
            {
                case "lights":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeLights(_bridge));
                    break;
                case "groups":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeGroups(_bridge));
                    break;
                case "rules":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeRules(_bridge));
                    break;
                case "resourcelinks":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeResourceLinks(_bridge));
                    break;
                case "scenes":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeScenes(_bridge));
                    break;
                case "sensors":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeSensors(_bridge));
                    break;
                case "schedules":
                    ListActionHueObjects.AddRange(HueObjectHelper.GetBridgeSchedules(_bridge));
                    break;
                default:
                    break;
            }
        }

        public void SetBridge(Bridge bridge)
        {
            _bridge = bridge;
        }
    }
}