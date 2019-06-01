using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.Rules.Validation;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Utils;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Rules.Creator
{
    public class RuleCreatorViewModel: ValidatableBindableBase
    {
        private string _name;
        private bool _enabled;
        private Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings _bs;
        private Bridge _bridge;
        public RuleCreatorViewModel()
        {
            _name = string.Empty;
            _enabled = true;
            _listRuleActions = new ObservableCollection<RuleAction>();
            _listAvailableHueObject = new List<IHueObject>();
            _listHueObjects = new List<IHueObject>();
            _listRuleConditions = new ObservableCollection<RuleCondition>();

            
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            List<IHueObject> objects = await _bridge.GetAllObjectsAsync(WinHueSettings.settings.ShowHiddenScenes,true);
            _bs = await _bridge.GetBridgeSettingsAsyncTask();          
            _listAvailableHueObject.AddRange(objects);

        }

        #region RuleCreator

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Rule Rule
        {
            get
            {
                Rule newrule = new Rule
                {
                    name = Name,
                    actions = ListRuleActions.ToList(),
                    conditions = ListRuleConditions.ToList(),
                    status = Enabled ? "enabled" : "disabled"
                };
                return newrule;
            }
            set
            {
                Name = value.name;
                Enabled = value.status == "enabled";
                ListRuleConditions = new ObservableCollection<RuleCondition>(value.conditions);
                ListRuleActions = new ObservableCollection<RuleAction>(value.actions);
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        #endregion

        #region RuleAction

        private ObservableCollection<RuleAction> _listRuleActions;
        private RuleAction _selectedRuleAction;
        private object _actionProperties;
        private List<IHueObject> _listHueObjects;
        private IHueObject _selectedHueObject;
        private string _currentPath;
        private Type _selectedHueObjectType;
        private readonly List<IHueObject> _listAvailableHueObject;

        public ICommand Action_AddActionCommand => new RelayCommand(param => AddAction(), (param) => CanAddAction());
        public ICommand Action_RemoveRuleActionCommand => new RelayCommand(param => RemoveRuleAction(), (param) => CanRemoveRuleAction());
        public ICommand Action_SelectRuleActionCommand => new RelayCommand(param => SelectRuleAction());
        public ICommand Action_SelectActionObjectCommand => new RelayCommand(param => SelectActionObject(), (param) => CanSelectActionObject());
        public ICommand Action_SelectHueObjectTypeCommand => new RelayCommand(param => SelectHueObjectType(), (param) => CanSelectHueObjectType());
        public ICommand Action_MoveUpRuleActionCommand => new RelayCommand(param => MoveUpRuleAction(), (param) => CanMoveUpRuleAction());
        public ICommand Action_MoveDownRuleActionCommand => new RelayCommand(param => MoveDownRuleAction(), (param) => CanMoveDownRuleAction());
        public ICommand Action_ClearRuleActionCommand => new RelayCommand(param => ClearRuleAction(),(param) => CanClearRuleAction());

        private bool CanClearRuleAction()
        {
            return CanMoveRuleAction();
        }

        private bool CanMoveRuleAction()
        {
            return SelectedRuleAction != null;
        }

        private bool CanRemoveRuleAction()
        {
            return CanMoveRuleAction();
        }

        private void ClearRuleAction()
        {
            ActionProperties = null;
            SelectedHueObjectType = null;
            SelectedRuleAction = null;
        }

        private bool CanMoveUpRuleAction()
        {
            if (!CanMoveRuleAction()) return false;
            if (_listRuleActions.IndexOf(SelectedRuleAction) == 0) return false;
            return true;
        }

        private bool CanMoveDownRuleAction()
        {
            if (!CanMoveRuleAction()) return false;
            if (_listRuleActions.IndexOf(SelectedRuleAction) == (_listRuleActions.Count -1)) return false;
            return true;

        }

        private bool CanSelectActionObject()
        {
            return SelectedRuleAction == null;
        }

        private void MoveUpRuleAction()
        {
            int index = _listRuleActions.IndexOf(SelectedRuleAction);

            if (index > -1)
            {
                RuleAction ra = SelectedRuleAction;
                _listRuleActions.RemoveAt(index);
                _listRuleActions.Insert(index -1, ra);
            }
        }

        private void MoveDownRuleAction()
        {
            int index = _listRuleActions.IndexOf(SelectedRuleAction);

            if (index > -1)
            {
                RuleAction ra = SelectedRuleAction;
                _listRuleActions.RemoveAt(index);
                _listRuleActions.Insert(index + 1, ra);
            }
        }

        private void SelectActionObject()
        {
            if (_selectedHueObject == null) return;
            if (_selectedHueObjectType == typeof(Light))
            {
                ActionProperties = new State();
                CurrentPath = $"/lights/{SelectedHueObject.Id}/state";
            }
            else if (_selectedHueObjectType == typeof(Group))
            {
                ActionProperties = new Action();
                CurrentPath = $"/groups/{SelectedHueObject.Id}/action";
            }
            else if (_selectedHueObjectType == typeof(Sensor))
            {
                Sensor s = (Sensor)_selectedHueObject;
                CurrentPath = $"/sensors/{SelectedHueObject.Id}/state";
                ActionProperties = HueSensorStateFactory.CreateSensorStateFromSensorType(s.type);
            }
            else if (_selectedHueObjectType == typeof(Scene))
            {
                CurrentPath = $"/groups/0/action";
                SceneBody sb = new SceneBody()
                {
                    scene = SelectedHueObject.Id
                };
                ActionProperties = sb;

            }
            else if(_selectedHueObjectType == typeof(Schedule))
            {
                ActionProperties = new Schedule();
                CurrentPath = $"/schedules/{SelectedHueObject.Id}";
                
            }

        }

        private bool CanSelectHueObjectType()
        {
            return SelectedRuleAction == null;
        }

        private void SelectHueObjectType()
        {

            switch (SelectedHueObjectType)
            {
                case Type light when light == typeof(Light):
                    ListHueObjects = _listAvailableHueObject.OfType<Light>().ToList<IHueObject>();

                    break;
                case Type group when group == typeof(Group):
                    ListHueObjects = _listAvailableHueObject.OfType<Group>().ToList<IHueObject>();

                    break;
                case Type sensor when sensor == typeof(Sensor):
                    ListHueObjects = _listAvailableHueObject.OfType<Sensor>().ToList<IHueObject>();

                    break;
                case Type scene when scene == typeof(Scene):
                    ListHueObjects = _listAvailableHueObject.OfType<Scene>().ToList<IHueObject>();

                    break;
                case Type schedule when schedule == typeof(Schedule):
                    ListHueObjects = _listAvailableHueObject.OfType<Schedule>().ToList<IHueObject>();
                    break;
                default:
                    ListHueObjects = null;
                    break;

            }
            ActionProperties = null;
        }

        private void SelectRuleAction()
        {
            if (SelectedRuleAction == null) return;
            HueAddress ha = SelectedRuleAction.address;
            CurrentPath = SelectedRuleAction.address;
            switch (ha.objecttype)
            {
                case "lights":
                    SelectedHueObjectType = typeof(Light);
                    SelectHueObjectType();
                    CurrentPath = SelectedRuleAction.address;
                    if (_listHueObjects.Exists(x => x.Id == ha.id))
                    {
                        SelectedHueObject = _listHueObjects.Find(x => x.Id == ha.id);
                        ActionProperties = Serializer.DeserializeToObject<State>(SelectedRuleAction.body);
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                case "groups":
                    if (SelectedRuleAction.body.Contains("scene")) goto case "scenes";

                    SelectedHueObjectType = typeof(Group);
                    SelectHueObjectType();
                    if (_listHueObjects.Exists(x => x.Id == ha.id))
                    {
                        SelectedHueObject = _listHueObjects.Find(x => x.Id == ha.id);
                        ActionProperties = Serializer.DeserializeToObject<Action>(SelectedRuleAction.body);
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                case "sensors":
                    SelectedHueObjectType = typeof(Sensor);
                    SelectHueObjectType();
                    if (_listHueObjects.Exists(x => x.Id == ha.id))
                    {
                        SelectedHueObject = _listHueObjects.Find(x => x.Id == ha.id);
                        Sensor s = (Sensor)SelectedHueObject;

                        ActionProperties = Serializer.DeserializeToObject(SelectedRuleAction.body,
                            HueSensorStateFactory.CreateSensorStateFromSensorType(s.type).GetType());

                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                case "scenes":
                    SelectedHueObjectType = typeof(Scene);
                    SelectHueObjectType();
                    SceneBody sb = Serializer.DeserializeToObject<SceneBody>(SelectedRuleAction.body);
                    if (_listHueObjects.Exists(x => x.Id == sb.scene))
                    {
                        SelectedHueObject = _listHueObjects.Find(x => x.Id == sb.scene);
                        ActionProperties = sb;
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "schedules":
                    SelectedHueObjectType = typeof(Schedule);
                    SelectHueObjectType();
                    if (_listHueObjects.Exists(x => x.Id == ha.id))
                    {
                        Schedule sc = Serializer.DeserializeToObject<Schedule>(SelectedRuleAction.body);
                        SelectedHueObject = _listHueObjects.Find(x => x.Id == ha.id);
                        ActionProperties = sc;
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                default:
                    MessageBox.Show(GlobalStrings.Rule, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }


        }

        private void RemoveRuleAction()
        {

            if (SelectedRuleAction != null)
                SelectedHueObjectType = null;
            ListRuleActions.Remove(SelectedRuleAction);
            SelectedRuleAction = null;

        }

        private bool CanAddAction()
        {
            if (ListRuleActions.Count > 7) return false;
            if (Serializer.SerializeJsonObject(ActionProperties) == "{}") return false;
            return true;
        }

        private void AddAction()
        {
            DialogResult result = DialogResult.Yes;

            RuleAction ra = new RuleAction();
            HueAddress address = new HueAddress(CurrentPath);

            if (ListRuleActions.Any(x => x.address == address))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ActionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleActions.Remove(ListRuleActions.FirstOrDefault(x => x.address == address));
            }

            if (result != DialogResult.Yes) return;
            ra.address = address;
            ra.method = "PUT";
            ra.body = Serializer.SerializeJsonObject(_actionProperties);
            ListRuleActions.Add(ra);
            ActionProperties = null;
            SelectedHueObjectType = null;
            


        }

        public ObservableCollection<RuleAction> ListRuleActions
        {
            get => _listRuleActions;
            set => SetProperty(ref _listRuleActions, value);
        }

        public object ActionProperties
        {
            get => _actionProperties;
            set => SetProperty(ref _actionProperties, value);
        }

        public RuleAction SelectedRuleAction
        {
            get => _selectedRuleAction;
            set => SetProperty(ref _selectedRuleAction, value);
        }

        public List<IHueObject> ListHueObjects
        {
            get => _listHueObjects;
            set => SetProperty(ref _listHueObjects, value);
        }

        public IHueObject SelectedHueObject
        {
            get => _selectedHueObject;
            set => SetProperty(ref _selectedHueObject, value);
        }

        public string CurrentPath
        {
            get => _currentPath;
            set => SetProperty(ref _currentPath, value);
        }

        public Type SelectedHueObjectType
        {
            get => _selectedHueObjectType;
            set => SetProperty(ref _selectedHueObjectType, value);
        }
#endregion

#region RuleCondition
        private List<HuePropertyTreeViewItem> _listConditionProperties;
        private string _conditionValue;
        private HuePropertyTreeViewItem _selectedConditionProperty;
        private string _conditionOperator;
        private ObservableCollection<RuleCondition> _listRuleConditions;
        private RuleCondition _selectedRuleCondition;
        private Type _selectedRuleConditionType;
        private List<IHueObject> _listConditionHueObjects;
        private IHueObject _selectedConditionHueObject;

        public ICommand Condition_AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand Condition_RemoveRuleConditionCommand => new RelayCommand(param => RemoveRuleCondition(), (param) => CanRemoveRuleCondition());
        public ICommand Condition_SelectRuleConditionCommand => new RelayCommand(param => SelectRuleCondition());
        public ICommand Condition_ClearSelectedRuleConditionCommand => new RelayCommand(param => ClearSelectedRuleCondition(), (param) => CanClearRuleCondition());
        public ICommand BtnEventCommand => new RelayCommand(InsertButtonValue, param => CanInsertButtonValue());

        private bool CanInsertButtonValue()
        {
            // TODO : check type
            return true;
        }

        private void InsertButtonValue(object param)
        {
            ConditionValue = param.ToString();
        }

        private bool CanClearRuleCondition()
        {
            return SelectedRuleCondition != null;
        }

        public ICommand Condition_SelectConditionObjectTypeCommand => new RelayCommand(param => SelectConditionObjectType(), (param) => CanSelectConditionObjectType());
        public ICommand Condition_SelectConditionHueObjectCommand => new RelayCommand(param => SelectConditionHueObject(), (param) => CanSelectConditionHueObject());

        private bool CanSelectConditionHueObject()
        {
            return SelectedRuleCondition == null;
        }

        private void SelectConditionHueObject()
        {
            ListConditionProperties = null;
            if (SelectedConditionHueObject == null) return;
            ListConditionProperties = TreeViewHelper.BuildPropertiesTree(SelectedConditionHueObject,$"/{SelectedConditionHueObject.GetType().Name.ToLower()+"s"}/{SelectedConditionHueObject.Id}", "object").ToList();

        }

        private bool CanSelectConditionObjectType()
        {
            return SelectedRuleCondition == null;
        }

        private void SelectConditionObjectType()
        {
            if(SelectedRuleConditionType == null) return;
            ListConditionHueObjects = null;
            ListConditionProperties = null;
            switch (SelectedRuleConditionType)
            {
                case Type light when light == typeof(Light):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Light>().ToList<IHueObject>();

                    break;
                case Type group when group == typeof(Group):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Group>().ToList<IHueObject>();

                    break;
                case Type sensor when sensor == typeof(Sensor):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Sensor>().ToList<IHueObject>();

                    break;
                case Type scene when scene == typeof(Scene):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Scene>().ToList<IHueObject>();

                    break;
                case Type sched when sched == typeof(Schedule):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Schedule>().ToList<IHueObject>();
                    break;
                case Type rule when rule == typeof(Rule):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Rule>().ToList<IHueObject>();
                    break;
                case Type rl when rl == typeof(Resourcelink):
                    ListConditionHueObjects = _listAvailableHueObject.OfType<Resourcelink>().ToList<IHueObject>();
                    break;
                case Type brs when brs == typeof(Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings):
                    ListConditionProperties = TreeViewHelper.BuildPropertiesTree(_bs, "/config", "Config").ToList();
                    break;
                default:
                    ListConditionHueObjects = null;
                    break;

            }
        }

        private void ClearSelectedRuleCondition()
        {
            SelectedRuleCondition = null;
        }

        private void SelectRuleCondition()
        {
            if (_selectedRuleCondition == null) return;

            SelectedRuleConditionType = _selectedRuleCondition.address.objecttype == "config" ? typeof(Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings) : HueObjectCreator.CreateHueObject(_selectedRuleCondition.address.objecttype).GetType();
            SelectConditionObjectType();

            if (_selectedRuleCondition.address.objecttype != "config")
            {
                SelectedConditionHueObject = ListConditionHueObjects.FirstOrDefault(x => x.Id == _selectedRuleCondition.address.id);
                SelectConditionHueObject();
            }

            if (SelectedConditionHueObject != null || SelectedRuleConditionType == typeof(Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings))
            {
                ConditionOperator = _selectedRuleCondition.@operator;
                ConditionValue = _selectedRuleCondition.value;
                foreach (HuePropertyTreeViewItem i in ListConditionProperties)
                {
                    HuePropertyTreeViewItem hti = ParseRuleForProperty(_selectedRuleCondition.address.ToString(), i);
                    if (hti != null)
                    {
                        SelectedConditionProperty = hti;
                        break;
                    }

                }

            }
            else
            {
                MessageBox.Show(GlobalStrings.Rule_SelectedObjectDoesNotExists, GlobalStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ExpandAllNodes(HuePropertyTreeViewItem treeItem, string path)
        {
            treeItem.IsExpanded = path.Contains(treeItem.Address.ToString());
            foreach (var childItem in treeItem.Items.OfType<HuePropertyTreeViewItem>())
            {
                ExpandAllNodes(childItem, path);
            }
        }

        private HuePropertyTreeViewItem ParseRuleForProperty(string address, HuePropertyTreeViewItem rtvi)
        {
            rtvi.IsSelected = false;
            if (rtvi.Address == new HueAddress(address))
            {
                rtvi.IsSelected = true;
                return rtvi;
            }
            if (rtvi.Items.Count == 0) return null;

            foreach (HuePropertyTreeViewItem r in rtvi.Items)
            {
                HuePropertyTreeViewItem nrt = ParseRuleForProperty(address, r);
                if (nrt == null) continue;
                nrt.IsSelected = true;
                return nrt;
            }



            return null;
        }

        private bool CanRemoveRuleCondition()
        {
            return SelectedRuleCondition != null;
        }

        private void RemoveRuleCondition()
        {
            ListRuleConditions.Remove(SelectedRuleCondition);
            SelectedRuleCondition = null;
        }

        private bool CanAddCondition()
        {

            if (ListRuleConditions.Count > 7) return false;
            if (ConditionOperator == null) return false;
            if (SelectedConditionProperty == null) return false;
            if (SelectedConditionProperty.Items.Count > 0) return false;
            if (!ConditionValue.IsValid() && ConditionOperator != "dx") return false;
            return true;
        }

        private void AddCondition()
        {
            /*DialogResult result = DialogResult.Yes;
            if (ListRuleConditions.Any(x => x.address.ToString().Equals(SelectedConditionProperty.Address)))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleConditions.Remove(
                        ListRuleConditions.FirstOrDefault(x => x.address.ToString().Equals(SelectedConditionProperty.Address)));
            }
            if (result == DialogResult.Yes)*/
                ListRuleConditions.Add(new RuleCondition() { address = SelectedConditionProperty.Address, @operator = ConditionOperator, value = ConditionOperator == "dx" ? null : ConditionValue });
        }

        public List<HuePropertyTreeViewItem> ListConditionProperties
        {
            get => _listConditionProperties;
            set => SetProperty(ref _listConditionProperties, value);
        }

        [RuleValueValidation]
        public string ConditionValue
        {
            get => _conditionValue;
            set => SetProperty(ref _conditionValue, value);
        }

        public HuePropertyTreeViewItem SelectedConditionProperty
        {
            get => _selectedConditionProperty;
            set => SetProperty(ref _selectedConditionProperty, value);
        }

        public string ConditionOperator
        {
            get => _conditionOperator;
            set
            {
                SetProperty(ref _conditionOperator, value);
                if (value == "dx")
                    ConditionValue = string.Empty;
            }
        }

        public ObservableCollection<RuleCondition> ListRuleConditions
        {
            get => _listRuleConditions;
            set => SetProperty(ref _listRuleConditions, value);
        }

        public RuleCondition SelectedRuleCondition
        {
            get => _selectedRuleCondition;
            set => SetProperty(ref _selectedRuleCondition, value);
        }

        public Type SelectedRuleConditionType
        {
            get => _selectedRuleConditionType;
            set => SetProperty(ref _selectedRuleConditionType,value);
        }

        public List<IHueObject> ListConditionHueObjects
        {
            get => _listConditionHueObjects;
            set => SetProperty(ref _listConditionHueObjects,value);
        }

        public IHueObject SelectedConditionHueObject
        {
            get => _selectedConditionHueObject;
            set => SetProperty(ref _selectedConditionHueObject,value);
        }

#endregion
    }
}

