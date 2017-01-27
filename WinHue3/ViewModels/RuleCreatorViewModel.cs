using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using Action = HueLib2.Action;
using MessageBox = System.Windows.MessageBox;
using System.ComponentModel;

namespace WinHue3.ViewModels
{
    public class RuleCreatorViewModel : ValidatableBindableBase
    {
        private Rule _rule;
        private List<HueObject> _listDataStore;
        private List<HueObject> _listcbsensors;
        private HueObject _selectedSensor;
        private PropertyInfo _selectedPropertyInfo;
        private ObservableCollection<RuleCondition> _listboxConditions;
        private string _selectedOperator;
        private string _conditionvalue = string.Empty;
        private ObservableCollection<RuleAction> _listboxActions;
        private RuleCondition _selectedCondition;
        private string _selectedActionType = null;
        private HueObject _selectedActionObject;
        private PropertyInfo _selectedActionProperty;
        private string _actionPropertyValue = string.Empty;
        private ObservableCollection<KeyValuePair<PropertyInfo, dynamic>> _listboxActionPropertyInfos;
        private KeyValuePair<PropertyInfo, dynamic> _selectedProperty;
        private RuleAction _selectedAction;

        #region CTOR

        public RuleCreatorViewModel()
        {

        }

        public void Initialize(List<HueObject> listObjects)
        {
            _rule = new Rule();
            _listDataStore = listObjects;
            _listcbsensors = listObjects.OfType<Sensor>().ToList<HueObject>();
            _listboxConditions = new ObservableCollection<RuleCondition>();
            _listboxActions = new ObservableCollection<RuleAction>();
            _listboxActionPropertyInfos = new ObservableCollection<KeyValuePair<PropertyInfo, dynamic>>();
           // SetError(GlobalStrings.Rule_NoCondition, "ListConditions");
           // SetError(GlobalStrings.Rule_NoAction, "ListActions");
          //  SetError(GlobalStrings.Rule_NameError, "RuleName");
        }

        public void Initialize(List<HueObject> listObjects, HueObject modifiedRule)
        {
            _rule = (Rule)modifiedRule;
            _listDataStore = listObjects;
            _listcbsensors = listObjects.OfType<Sensor>().ToList<HueObject>();
            _listboxConditions = new ObservableCollection<RuleCondition>(_rule.conditions);
            _listboxActions = new ObservableCollection<RuleAction>(_rule.actions);
            _listboxActionPropertyInfos = new ObservableCollection<KeyValuePair<PropertyInfo, dynamic>>();
        }

        #endregion

        #region PROPERTIES

        public KeyValuePair<PropertyInfo, dynamic> SelectedProperty
        {
            get { return _selectedProperty; }
            set { SetProperty(ref _selectedProperty,value);}
        }

        public RuleAction SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                SetProperty(ref _selectedAction,value);
                if (_selectedAction != null)
                    FillActionPropertyFromAction();
            }
        }

        public ObservableCollection<KeyValuePair<PropertyInfo, dynamic>> ListProperties => _listboxActionPropertyInfos;

        public string ActionPropertyValue
        {
            get { return _actionPropertyValue; }
            set {
                SetProperty(ref _actionPropertyValue,value);
                OnPropertyChanged("CanAddProperty");
            }
        }

        public Visibility ValueVisible
        {
            get
            {
                if (_selectedActionProperty == null) return Visibility.Visible;
                if (_selectedActionProperty.Name != "scene") return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        public PropertyInfo SelectedActionProperty
        {
            get { return _selectedActionProperty; }
            set
            {
                SetProperty(ref _selectedActionProperty,value);
                OnPropertyChanged("CanAddProperty");
                OnPropertyChanged("ValueVisible");
            }
        }



        public List<HueObject> ListActionObjects
        {
            get
            {
                if (_selectedActionType == null) return null;
                switch (_selectedActionType)
                {
                    case "lights":
                        return _listDataStore.OfType<Light>().ToList<HueObject>();
                    case "groups":
                        return _listDataStore.OfType<Group>().ToList<HueObject>();
                    case "scenes":
                        return _listDataStore.OfType<Scene>().ToList<HueObject>();
                    case "sensors":
                        return _listDataStore.OfType<Sensor>().ToList<HueObject>();
                    case "schedules":
                        return _listDataStore.OfType<Schedule>().ToList<HueObject>();
                    default:
                        return null;

                }
            }
        }

        public string SelectedActionType
        {
            get { return _selectedActionType; }
            set
            {
                SetProperty(ref _selectedActionType,value);
                OnPropertyChanged("ListActionObjects");
                OnPropertyChanged("CanAddProperty");
            }
        }

        public bool CanChangeAction => _listboxActionPropertyInfos.Count == 0;

        public HueObject SelectedActionObject
        {
            get { return _selectedActionObject; }
            set
            {
                SetProperty(ref _selectedActionObject,value);
                OnPropertyChanged("ListActionProperties");
                OnPropertyChanged("CanAddProperty");
            }
        }

        public PropertyInfo[] ListActionProperties
        {
            get
            {
                if (_selectedActionType == null) return null;
                switch (_selectedActionType)
                {
                    case "lights":
                        return new State().GetType().GetProperties();
                    case "groups":
                        return new Action().GetType().GetProperties();
                    case "scenes":
                        return new SceneBody().GetType().GetProperties();
                    case "sensors":
                        if (_selectedActionObject == null) return new PropertyInfo[0];
                        PropertyInfo[] listnewprop = ((Sensor) _selectedActionObject).state.GetType().GetProperties();
                        int index = listnewprop.FindIndex(x => x.Name == "lastupdated");
                        if (index != -1)
                            listnewprop = listnewprop.RemoveAt(index);
                        return listnewprop;
                    case "schedules":
                        return new ScheduleBody().GetType().GetProperties();
                    default:
                        return null;
                }
            }
        }




        public RuleCondition SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                _selectedCondition = value;
                OnPropertyChanged();
                if (_selectedCondition == null) return;
                RuleCondition rc = (RuleCondition) _selectedCondition;
                _conditionvalue = rc.value ?? string.Empty;
                OnPropertyChanged("ConditionValue");
                string[] sensorinfo = rc.address.Split('/');
                _selectedSensor = ListSensors.Find(x => x.Id == sensorinfo[2]);
                OnPropertyChanged("SelectedSensor");
                OnPropertyChanged("ListSensorProperties");
                _selectedPropertyInfo = ListSensorProperties.First(x => x.Name == sensorinfo[4]);
                OnPropertyChanged("SelectedPropertyInfo");
                _selectedOperator = rc.op;
                OnPropertyChanged("SelectedOperator");
                OnPropertyChanged("CanDeleteCondition");
            }
        }

        public Visibility IsOperatorChanged
        {
            get
            {
                if (_selectedOperator == string.Empty) return Visibility.Visible;
                return _selectedOperator == "dx" ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        public bool RuleEnabled
        {
            get
            {
                if (_rule.status == null) return true;
                return _rule.status == "enabled";
            }
            set { _rule.status = value ? "enabled" : "disabled"; }
        }

        public string RuleName
        {
            get { return _rule.name ?? string.Empty; }
            set
            {
                if (value == string.Empty)
                {
                    _rule.name = null;
                  //  SetError(GlobalStrings.Rule_NameError);
                }
                else
                {
                    _rule.name = value;
                    //RemoveError(GlobalStrings.Rule_NameError);
                }
                
                OnPropertyChanged();
                
            }
        }

        public string ConditionValue
        {
            get { return _conditionvalue; }
            set
            {
                SetProperty(ref _conditionvalue,value);
                OnPropertyChanged("CanAddCondition");
            }
        }

        public string SelectedOperator
        {
            get { return _selectedOperator; }
            set
            {
                SetProperty(ref _selectedOperator,value);
                OnPropertyChanged("CanAddCondition");
                OnPropertyChanged("IsOperatorChanged");
            }
        }

        public ObservableCollection<RuleCondition> ListConditions => _listboxConditions;
        public ObservableCollection<RuleAction> ListActions => _listboxActions;

        public PropertyInfo SelectedPropertyInfo
        {
            get { return _selectedPropertyInfo; }
            set
            {
                SetProperty(ref _selectedPropertyInfo,value);
                OnPropertyChanged("CanAddCondition");
                OnPropertyChanged("CanAddProperty");
            }
        }

        public HueObject SelectedSensor
        {
            get { return _selectedSensor; }
            set
            {
                SetProperty(ref _selectedSensor,value);
                OnPropertyChanged();
                OnPropertyChanged("ListSensorProperties");
                OnPropertyChanged("CanAddCondition");
            }

        }

        public List<HueObject> ListSensors => _listcbsensors;

        public PropertyInfo[] ListSensorProperties => _selectedSensor?.GetType().GetProperty("state").GetValue(_selectedSensor).GetType().GetProperties();


        #endregion

        #region METHODS

        private void AddCondition()
        {
            if (_listboxConditions.Count == 8)
            {
                MessageBox.Show(GlobalStrings.Rule_MaxConditions, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RuleCondition rc = new RuleCondition() {address = $"/sensors/{_selectedSensor.Id}/state/{_selectedPropertyInfo.Name}", op = _selectedOperator, value = _selectedOperator != "dx" ?_conditionvalue : null};
            if (_listboxConditions.Any(x =>x.address == rc.address))
            {
                if (MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                int index = _listboxConditions.FindIndex(x => x.address == rc.address);
                if (index == -1) return;
                _listboxConditions.RemoveAt(index);
            }

            _listboxConditions.Add(rc);
            //RemoveError(GlobalStrings.Rule_NoCondition, "ListConditions");
            ResetConditionFields();
        }

        private void ResetConditionFields()
        {
            _selectedOperator = null;
            OnPropertyChanged("SelectedOperator");
            _selectedSensor = null;
            OnPropertyChanged("SelectedSensor");
            _selectedPropertyInfo = null;
            OnPropertyChanged("SelectedPropertyInfo");
            _conditionvalue = string.Empty;
            OnPropertyChanged("ConditionValue");
            OnPropertyChanged("CanAddCondition");
        }

        private void ConditionDelete()
        {
            if (_selectedCondition == null) return;
            _listboxConditions.Remove(_selectedCondition);
            OnPropertyChanged("CanDeleteCondition");
            ResetConditionFields();
            if (_listboxConditions.Count != 0) return;
            //SetError(GlobalStrings.Rule_NoCondition, "ListConditions");
        }

        public Rule GetRule()
        {
            _rule.conditions = new List<RuleCondition>(_listboxConditions);
            _rule.actions = new List<RuleAction>(_listboxActions);
            return _rule;
        }

        public void AddProperty()
        {
            PropertyInfo pi = _selectedActionProperty;
            Type type = pi.PropertyType;
            if (Nullable.GetUnderlyingType(pi.PropertyType) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (_selectedActionProperty.Name == "scene") _actionPropertyValue = _selectedActionObject.Id;

            dynamic value;
            CastToType(type, _actionPropertyValue, out value);
            if (value == null) return;

            if (CheckIfPropertySet(pi))
            {
                if (MessageBox.Show(string.Format(GlobalStrings.Rule_PropertyAlreadyExists, pi.Name), GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                _listboxActionPropertyInfos.Remove(_listboxActionPropertyInfos.First(x => x.Key == pi));
                _listboxActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(pi, value));
                ResetActionPropertyFields();
            }
            else
            {
                _listboxActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(pi, value));
                ResetActionPropertyFields();
            }
        }

        private void ResetActionPropertyFields()
        {
            OnPropertyChanged("CanChangeAction");
            _selectedActionProperty = null;
            _actionPropertyValue = string.Empty;
            OnPropertyChanged("ListActionObjects");
            OnPropertyChanged("ListProperties");
            OnPropertyChanged("SelectedActionType");
            OnPropertyChanged("SelectedActionProperty");
            OnPropertyChanged("SelectedActionObject");
            OnPropertyChanged("ActionPropertyValue");
            OnPropertyChanged("CanAddProperty");
            OnPropertyChanged("CanAddAction");
        }

        private bool CheckIfPropertySet(PropertyInfo prop)
        {
            return _listboxActionPropertyInfos.Any(kvp => kvp.Key == prop);
        }

        private static void CastToType(Type type, string text, out object result)
        {
            try
            {
                if (type == typeof(byte))
                {
                    result = byte.Parse(text);
                    return;
                }

                if (type == typeof(int))
                {
                    result = int.Parse(text);
                    return;
                }

                if (type == typeof(ushort))
                {
                    result = ushort.Parse(text);
                    return;
                }

                if (type == typeof(bool))
                {
                    result = bool.Parse(text);
                    return;
                }

                if (type == typeof(float))
                {
                    result = float.Parse(text);
                    return;
                }

                if (type == typeof(string))
                {
                    if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
                    {
                        result = text;
                        return;
                    }
                }

                if (type == typeof(uint))
                {
                    result = uint.Parse(text);
                    return;

                }

                if (type == typeof (XY))
                {
                    result = XY.Parse(text);
                    return;
                }

                if (type == typeof (short))
                {
                    result = short.Parse(text);
                    return;
                }
                result = null;
            }
            catch (FormatException)
            {
                result = null;
                MessageBox.Show(GlobalStrings.Rule_NotProperFormat + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException)
            {
                result = null;
                MessageBox.Show(GlobalStrings.Rule_ValueNotInRange + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAction()
        {

            if (_listboxActions.Count == 8)
            {
                MessageBox.Show(GlobalStrings.Rule_MaxAction, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RuleAction action = new RuleAction() {method = "PUT"};
            switch (_selectedActionType)
            {
                case "lights":
                    action.address = $@"/lights/{_selectedActionObject.Id}/state";
                    action.body = FillPropertiesFromList<State>();
                    break;
                case "groups":
                    action.address = $@"/groups/{_selectedActionObject.Id}/action";
                    action.body = FillPropertiesFromList<Action>();
                    break;
                case "scenes":
                    action.address = "/groups/0/action";
                    action.body = new SceneBody() {scene = _listboxActionPropertyInfos.First().Value};
                    break;
                case "sensors":
                    action.address = $@"/sensors/{_selectedActionObject.Id}/state";
                    action.body = FillPropertiesFromList(((Sensor) _selectedActionObject).state.GetType());
                    break;
                case "schedules":
                    action.address = $@"/schedules/{_selectedActionObject.Id}";
                    action.body = FillPropertiesFromList<ScheduleBody>();
                    break;
                default:
                    break;
            }

            if (_listboxActions.Any(x => x.address == action.address))
            {
                if(MessageBox.Show(GlobalStrings.Rule_ActionAlreadyExists, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                int index = _listboxActions.FindIndex(x => x.address == action.address);
                if (index == -1) return;
                _listboxActions.RemoveAt(index);
            }
                
            _listboxActions.Add(action);
            //RemoveError(GlobalStrings.Rule_NoAction, "ListActions");
            _selectedActionObject = null;
            _selectedActionType = null;
            _listboxActionPropertyInfos.Clear();
            ResetActionPropertyFields();
                
        }

        private RuleBody FillPropertiesFromList(Type sensorStateType)
        {
            object obj = Activator.CreateInstance(sensorStateType);
            foreach (KeyValuePair<PropertyInfo, dynamic> prop in _listboxActionPropertyInfos)
            {
                if (obj.HasProperty(prop.Key.Name))
                {
                    obj.GetType().GetProperty(prop.Key.Name).SetValue(obj, prop.Value);
                }
            }
            return (RuleBody) obj;

        }

        private T FillPropertiesFromList<T>() where T : new()
        {
            T result = new T();

            foreach (KeyValuePair<PropertyInfo, dynamic> prop in _listboxActionPropertyInfos)
            {
                PropertyInfo pi = result.GetType().GetProperty(prop.Key.Name);
                pi?.SetValue(result, prop.Value);
            }

            return result;
        }

        private void DeleteAction()
        {
            _listboxActions.Remove(_selectedAction);
            if (_listboxActions.Count == 0)
            {
                //SetError(GlobalStrings.Rule_NoAction, "ListAction");
                OnPropertyChanged("CanChangeAction");

            }
            
        }

        private void DeleteProperty()
        {
            _listboxActionPropertyInfos.Remove(_selectedProperty);
            if (_listboxActionPropertyInfos.Count == 0)
            {
                OnPropertyChanged("CanChangeAction");
                OnPropertyChanged("CanAddAction");
            }
        }

        private void FillActionPropertyFromAction()
        {
            if (_selectedAction == null) return;
            _listboxActionPropertyInfos.Clear();
            PropertyInfo[] prop = _selectedAction.body.GetType().GetProperties();
            foreach (PropertyInfo p in prop)
            {
                if( p.GetValue(_selectedAction.body) == null) continue; 
                _listboxActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(p,p.GetValue(_selectedAction.body)));
            }
            string[] adr = _selectedAction.address.Split('/');
            SelectedActionType = adr[1];
            SelectedActionObject = ListActionObjects.Find(x => x.Id == adr[2]);
            if (SelectedActionObject == null)
            {
                _selectedActionObject = null;
                _selectedActionType = null;
                ResetActionPropertyFields();
            }
            OnPropertyChanged("CanChangeAction");
            OnPropertyChanged("CanAddAction");
        }

        #endregion



        private bool CanAddCondition()
        {
            if (_listboxConditions?.Count == 8) return false;
            if (_selectedSensor == null) return false;
            if (_selectedOperator == null) return false;
            if (_selectedPropertyInfo == null) return false;
            if (_selectedOperator.Equals("dx")) return true;
            return _conditionvalue != string.Empty;            
        }

        private bool CanDeleteCondition()
        {
            return _selectedCondition != null;
        }

        private bool CanAddProperty()
        {
            if (_selectedActionProperty == null) return false;
            if (_selectedActionObject == null) return false;
            if (_selectedActionType == null) return false;
            if (_selectedActionType == string.Empty) return false;
            if (_selectedActionType == "scenes" && _selectedActionObject is Scene && _selectedActionProperty.Name == "scene") return true;
            if (_actionPropertyValue == string.Empty) return false;
            return true;
        }

        private bool CanAddAction()
        {
            return _listboxActionPropertyInfos?.Count > 0;
        }

        #region COMMANDS

        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand DeleteConditionCommand => new RelayCommand(param => ConditionDelete(), (param) => CanDeleteCondition());
        public ICommand AddPropertyCommand => new RelayCommand(param => AddProperty(), (param)=> CanAddProperty());
        public ICommand AddActionCommand => new RelayCommand(param => AddAction(), (param)=> CanAddAction());
        public ICommand DeleteActionCommand => new RelayCommand(param => DeleteAction());
        public ICommand DeletePropertyCommand => new RelayCommand(param => DeleteProperty());
        #endregion
    }
}
