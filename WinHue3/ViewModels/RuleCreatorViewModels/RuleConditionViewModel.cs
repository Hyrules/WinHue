using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using HueLib2.Objects.HueObject;
using HueLib2.Objects.Rules;
using WinHue3.Validation;

namespace WinHue3.ViewModels.RuleCreatorViewModels
{
    public class RuleConditionViewModel : ValidatableBindableBase
    {
        private IHueObject _selectedSensor;
        private PropertyInfo _property;
        private string _operator;
        private string _value;
        private List<IHueObject> _listsensors;
        private RuleCondition _selectedCondition;
        private ObservableCollection<RuleCondition> _listConditions;
        private PropertyInfo[] _listSensorProperties;

        public RuleConditionViewModel()
        {
            ListSensors = new List<IHueObject>();     
            ListConditions = new ObservableCollection<RuleCondition>();
            Value = string.Empty;
        }

        public IHueObject SelectedSensor
        {
            get { return _selectedSensor; }
            set { SetProperty(ref _selectedSensor,value); }
        }

        [MinimumCount(1,ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Rule_NoCondition")]
        public ObservableCollection<RuleCondition> ListConditions
        {
            get { return _listConditions; }
            set { SetProperty(ref _listConditions, value); }
        }

        public List<IHueObject> ListSensors
        {
            get { return _listsensors; }
            set { SetProperty(ref _listsensors, value); }
        }

        public PropertyInfo Property
        {
            get { return _property; }
            set { SetProperty(ref _property,value); }
        }

        public string Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator,value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value,value); }
        }

        public PropertyInfo[] ListSensorProperties
        {
            get { return _listSensorProperties; }
            set { SetProperty(ref _listSensorProperties, value); }
        }

        public RuleCondition SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                SetProperty(ref _selectedCondition,value);
                if (SelectedCondition == null) return;
                RuleCondition rc = SelectedCondition;
                Value = rc.value ?? string.Empty;
                
                string classname = rc.address.objecttype.TrimEnd('s');
                classname = "HueLib2." + classname.First().ToString().ToUpper() + string.Join("", classname.Skip(1));
                Type type = Type.GetType($"{classname}, HueLib2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                SelectedSensor = ListSensors?.Find(x => x.Id == rc.address.id && x.GetType() == type);
                SelectSensor();
                Property = ListSensorProperties?.First(x => x.Name == rc.address.subprop);
                Operator = rc.@operator;
            }
        }

        private void AddCondition()
        {
            if (ListConditions.Count == 8)
            {
                MessageBox.Show(GlobalStrings.Rule_MaxConditions, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string ns = SelectedSensor.GetType().Namespace;

            string typename = SelectedSensor.GetType().ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
            RuleCondition rc = new RuleCondition()
            {         
                @operator = Operator,
                value = Operator != "dx" ? Value : null
            };

            if (SelectedSensor.Name == "config")
            {
                rc.address = new RuleAddress()
                {
                    objecttype = "config",
                    property = Property.Name
                };
            }
            else
            {
                rc.address = new RuleAddress()
                {
                    objecttype = typename,
                    id = SelectedSensor.Id,
                    property = "state",
                    subprop = Property.Name
                };
            }

            if (SelectedCondition != null) 
            {
                if (MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                int index = ListConditions.FindIndex(x => x == SelectedCondition);
                if (index == -1) return;
                ListConditions.RemoveAt(index);
                ListConditions.Insert(index,rc);
            }
            else
            {
                ListConditions.Add(rc);
            }
            
            ResetConditionFields();
            RaisePropertyChanged("ListConditions");
        }

        private void DeleteCondition()
        {
            ListConditions.Remove(SelectedCondition);
            ResetConditionFields();
        }

        private void ResetConditionFields()
        {
            Operator = null;
            SelectedSensor = null;
            Property = null;
            Value = string.Empty;
            SelectedCondition = null;
        }

        private void SelectSensor()
        {
            if (IsSensorSelected())
            {
                if (SelectedSensor is Light)
                {
                    ListSensorProperties = new []{SelectedSensor.GetType().GetProperty("state").GetValue(SelectedSensor).GetType().GetProperty("on")};
                }
                else
                {
                    ListSensorProperties = SelectedSensor.GetType().GetProperty("state").GetValue(SelectedSensor).GetType().GetProperties();
                }
                
                if (SelectedSensor.Name == "config")
                {
                    ListSensorProperties = ListSensorProperties.RemoveAt(2);
                }
            }
            else
            {
                ListSensorProperties = null;
            }
        }

        private bool CanAddCondition()
        {
            if (ListConditions.Count == 8) return false;
            if (SelectedSensor == null) return false;
            if (Operator == null) return false;
            if (Property == null) return false;
            if (Operator.Equals("dx")) return true;
            return Value != string.Empty;
        }

        private bool IsSensorSelected()
        {
            return SelectedSensor != null;
        }

        private bool CanDeleteCondition()
        {
            return SelectedCondition != null;
        }

        public ICommand DeleteConditionCommand => new RelayCommand(param => DeleteCondition(), (param) => CanDeleteCondition());
        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand SelectSensorCommand => new RelayCommand(param => SelectSensor());
        public ICommand ClearConditionCommand => new RelayCommand(param => ResetConditionFields());


    }
}
