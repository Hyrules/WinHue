using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using WinHue3.Validation;
using Action = HueLib2.Action;

namespace WinHue3.ViewModels
{
    public class RuleActionViewModel : ValidatableBindableBase
    {
        private string _selectedActionType = null;
        private HueObject _selectedActionObject;
        private PropertyInfo _selectedActionProperty;
        private string _actionPropertyValue = string.Empty;
        private ObservableCollection<KeyValuePair<PropertyInfo, dynamic>> _listActionPropertyInfos;
        private RuleAction _selectedAction;
        private ObservableCollection<RuleAction> _listActions;
        private KeyValuePair<PropertyInfo, dynamic> _selectedProperty;
        private List<HueObject> _listDataStore;
        private PropertyInfo[] _listActionProperties;
        private List<HueObject> _listActionObjects;
        public RuleActionViewModel()
        {
            ListActionPropertyInfos = new ObservableCollection<KeyValuePair<PropertyInfo, dynamic>>();   
            ListActions = new ObservableCollection<RuleAction>();
        }

        public ObservableCollection<KeyValuePair<PropertyInfo, dynamic>> ListActionPropertyInfos
        {
            get { return _listActionPropertyInfos; }
            set { SetProperty(ref _listActionPropertyInfos,value); }
        }

        public string ActionPropertyValue
        {
            get { return _actionPropertyValue; }
            set { SetProperty(ref _actionPropertyValue, value); }
        }

        public PropertyInfo SelectedActionProperty
        {
            get { return _selectedActionProperty; }
            set { SetProperty(ref _selectedActionProperty, value); }
        }

        public List<HueObject> ListActionObjects
        {
            get { return _listActionObjects;}
            set { SetProperty(ref _listActionObjects, value); }
        }

        [MinimumCount(1, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Rule_NoAction")]
        public ObservableCollection<RuleAction> ListActions
        {
            get { return _listActions; }
            set { SetProperty(ref _listActions, value); }
        }

        public List<HueObject> ListDataStore
        {
            get { return _listDataStore; }
            set { SetProperty(ref _listDataStore, value); }
        }

        public string SelectedActionType
        {
            get { return _selectedActionType; }
            set { SetProperty(ref _selectedActionType, value); }
        }

        public HueObject SelectedActionObject
        {
            get { return _selectedActionObject; }
            set { SetProperty(ref _selectedActionObject, value); }
        }

        public KeyValuePair<PropertyInfo, dynamic> SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                SetProperty(ref _selectedProperty, value);
                SelectedActionProperty = ListActionProperties.FirstOrDefault(x => x.Name == value.Key.Name);
                ActionPropertyValue = value.Value.ToString();
            }
        }

        public PropertyInfo[] ListActionProperties
        {
            get { return _listActionProperties; }
            set { SetProperty(ref _listActionProperties, value); }
        }

        public RuleAction SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                SetProperty(ref _selectedAction,value);
                if (SelectedAction != null)
                    FillActionPropertyFromAction();
            }
        }

        private RuleBody FillPropertiesFromList(Type sensorStateType)
        {
            object obj = Activator.CreateInstance(sensorStateType);
            foreach (KeyValuePair<PropertyInfo, dynamic> prop in ListActionPropertyInfos)
            {
                if (obj.HasProperty(prop.Key.Name))
                {
                    obj.GetType().GetProperty(prop.Key.Name).SetValue(obj, prop.Value);
                }
            }
            return (RuleBody)obj;

        }

        private void FillActionPropertyFromAction()
        {
            if (SelectedAction == null) return;
            ListActionPropertyInfos.Clear();
            PropertyInfo[] prop = SelectedAction.body.GetType().GetProperties();
            foreach (PropertyInfo p in prop)
            {
                if (p.GetValue(SelectedAction.body) == null) continue;
                ListActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(p, p.GetValue(SelectedAction.body)));
            }
            string[] adr = SelectedAction.address.Split('/');
            SelectedActionType = adr[2] == "0" ? "scenes" : adr[1];
            string id = adr[2] == "0" ? ((SceneBody)SelectedAction.body).scene : adr[2];
            SelectedActionObject = ListActionObjects.Find(x => x.Id == id);
            if (SelectedActionObject == null)
            {
                SelectedActionObject = null;
                SelectedActionType = null;
                ResetActionPropertyFields();
            }
        }

        private T FillPropertiesFromList<T>() where T : new()
        {
            T result = new T();

            foreach (KeyValuePair<PropertyInfo, dynamic> prop in ListActionPropertyInfos)
            {
                PropertyInfo pi = result.GetType().GetProperty(prop.Key.Name);
                pi?.SetValue(result, prop.Value);
            }

            return result;
        }

        private void DeleteAction()
        {
            ListActions.Remove(SelectedAction);
            SelectedActionObject = null;
            SelectedActionType = null;
            ListActionPropertyInfos.Clear();
            ResetActionPropertyFields();
        }

        private void DeleteProperty()
        {
            ListActionPropertyInfos.Remove(SelectedProperty);
        }

        public void AddProperty()
        {
            PropertyInfo pi = SelectedActionProperty;
            Type type = pi.PropertyType;
            if (Nullable.GetUnderlyingType(pi.PropertyType) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (SelectedActionProperty.Name == "scene") _actionPropertyValue = SelectedActionObject.Id;

            dynamic value;
            CastToType(type, _actionPropertyValue, out value);
            if (value == null) return;

            if (CheckIfPropertySet(pi))
            {
                if (MessageBox.Show(string.Format(GlobalStrings.Rule_PropertyAlreadyExists, pi.Name), GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                ListActionPropertyInfos.Remove(ListActionPropertyInfos.First(x => x.Key == pi));
                ListActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(pi, value));
                ResetActionPropertyFields();
            }
            else
            {
                ListActionPropertyInfos.Add(new KeyValuePair<PropertyInfo, dynamic>(pi, value));
                ResetActionPropertyFields();
            }
        }

        private bool CheckIfPropertySet(PropertyInfo prop)
        {
            return ListActionPropertyInfos.Any(kvp => kvp.Key == prop);
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

                if (type == typeof(XY))
                {
                    result = XY.Parse(text);
                    return;
                }

                if (type == typeof(short))
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

            if (ListActions.Count == 8)
            {
                MessageBox.Show(GlobalStrings.Rule_MaxAction, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RuleAction action = new RuleAction() { method = "PUT" };
            switch (SelectedActionType)
            {
                case "lights":
                    action.address = $@"/lights/{SelectedActionObject.Id}/state";
                    action.body = FillPropertiesFromList<State>();
                    break;
                case "groups":
                    action.address = $@"/groups/{SelectedActionObject.Id}/action";
                    action.body = FillPropertiesFromList<Action>();
                    break;
                case "scenes":
                    action.address = "/groups/0/action";
                    action.body = new SceneBody() { scene = ListActionPropertyInfos.First().Value };
                    break;
                case "sensors":
                    action.address = $@"/sensors/{SelectedActionObject.Id}/state";
                    action.body = FillPropertiesFromList(((Sensor)SelectedActionObject).state.GetType());
                    break;
                case "schedules":
                    action.address = $@"/schedules/{SelectedActionObject.Id}";
                    action.body = FillPropertiesFromList<ScheduleBody>();
                    break;
                default:
                    break;
            }

            if (ListActions.Any(x => x.address == action.address))
            {
                if (MessageBox.Show(GlobalStrings.Rule_ActionAlreadyExists, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                int index = ListActions.FindIndex(x => x.address == action.address);
                if (index == -1) return;
                ListActions.RemoveAt(index);
            }

            ListActions.Add(action);
            SelectedActionObject = null;
            SelectedActionType = null;
            ListActionPropertyInfos.Clear();
            ResetActionPropertyFields();
            OnPropertyChanged("ListActions");
        }

        private void SelectAction()
        {
            if (SelectedActionType == null) return;
            switch (SelectedActionType)
            {
                case "lights":
                    ListActionObjects = ListDataStore.OfType<Light>().ToList<HueObject>();
                    break;
                case "groups":
                    ListActionObjects = ListDataStore.OfType<Group>().ToList<HueObject>();
                    break;
                case "scenes":
                    ListActionObjects = ListDataStore.OfType<Scene>().ToList<HueObject>();
                    break;
                case "sensors":
                    ListActionObjects = ListDataStore.OfType<Sensor>().ToList<HueObject>();
                    break;
                case "schedules":
                    ListActionObjects = ListDataStore.OfType<Schedule>().ToList<HueObject>();
                    break;
                default:
                    break;
            }

        }

        private void SelectActionObject()
        {

            if (SelectedActionType == null) return;
            switch (SelectedActionType)
            {
                case "lights":
                    ListActionProperties = new State().GetType().GetProperties();
                    break;
                case "groups":
                    ListActionProperties = new Action().GetType().GetProperties();
                    break;
                case "scenes":
                    ListActionProperties = new SceneBody().GetType().GetProperties();
                    break;
                case "sensors":
                    if (SelectedActionObject == null) ListActionProperties = new PropertyInfo[0];
                    PropertyInfo[] listnewprop = ((Sensor)SelectedActionObject).state.GetType().GetProperties();
                    int index = listnewprop.FindIndex(x => x.Name == "lastupdated");
                    if (index != -1)
                        listnewprop = listnewprop.RemoveAt(index);
                    ListActionProperties = listnewprop;
                    break;
                case "schedules":
                    ListActionProperties = new ScheduleBody().GetType().GetProperties();
                    break;
                default:
                    break;
            }
        }

        private void ResetActionPropertyFields()
        {
            SelectedActionProperty = null;
            ActionPropertyValue = string.Empty;
        }

        private bool CanAddProperty()
        {
            if (SelectedActionProperty == null) return false;
            if (SelectedActionObject == null) return false;
            if (SelectedActionType == null) return false;
            if (SelectedActionType == string.Empty) return false;
            if (SelectedActionType == "scenes" && SelectedActionObject is Scene && SelectedActionProperty.Name == "scene") return true;
            if (ActionPropertyValue == string.Empty) return false;
            return true;
        }

        private bool CanDeleteProperty()
        {
            return SelectedActionProperty != null;
        }

        private bool CanAddAction()
        {
            return ListActionPropertyInfos.Count > 0;
        }

        private bool CanDeleteAction()
        {
            return SelectedAction != null;
        }

        private bool CanSelectAction()
        {
            return ListActionPropertyInfos.Count == 0;
        }

        public ICommand AddPropertyCommand => new RelayCommand(param => AddProperty(), (param) => CanAddProperty());
        public ICommand AddActionCommand => new RelayCommand(param => AddAction(), (param) => CanAddAction());
        public ICommand DeleteActionCommand => new RelayCommand(param => DeleteAction(), (param) => CanDeleteAction());
        public ICommand DeletePropertyCommand => new RelayCommand(param => DeleteProperty(), (param) => CanDeleteProperty());
        public ICommand SelectActionCommand => new RelayCommand(param => SelectAction(), (param) => CanSelectAction());
        public ICommand SelectActionObjectCommand => new RelayCommand(param => SelectActionObject(), (param) => CanSelectAction());


    }
}
