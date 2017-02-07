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
using WinHue3.Validation;

namespace WinHue3.Models
{
    public class RuleConditionViewModel : ValidatableBindableBase
    {
        private HueObject _selectedSensor;
        private PropertyInfo _property;
        private string _operator;
        private string _value;
        private List<HueObject> _listsensors;
        private RuleCondition _selectedCondition;
        private ObservableCollection<RuleCondition> _listConditions;
        private PropertyInfo[] _listSensorProperties;

        public RuleConditionViewModel()
        {
            ListSensors = new List<HueObject>();     
            ListConditions = new ObservableCollection<RuleCondition>();
            Value = string.Empty;
        }

        public HueObject SelectedSensor
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

        public List<HueObject> ListSensors
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
                string[] sensorinfo = rc.address.Split('/');
                string classname = sensorinfo[1].TrimEnd('s');
                classname = "HueLib2." + classname.First().ToString().ToUpper() + string.Join("", classname.Skip(1));
                Type type = Type.GetType($"{classname}, HueLib2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                SelectedSensor = ListSensors?.Find(x => x.Id == sensorinfo[2] && x.GetType() == type);
                SelectSensor();
                Property = ListSensorProperties?.First(x => x.Name == sensorinfo[4]);
                Operator = rc.op;
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

            string typename = string.Empty;
            if (SelectedSensor.GetType().BaseType == typeof(HueObject))
            {
                typename = SelectedSensor.GetType().ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
            }
            else
            {
                typename = SelectedSensor.GetType().BaseType.ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
            }

            RuleCondition rc = new RuleCondition()
            {
                address = SelectedSensor.GetName() != "config" ? $"/{typename}/{SelectedSensor.Id}/state/{Property.Name}" : $"/{SelectedSensor.Id}/{Property.Name}",
                op = Operator,
                value = Operator != "dx" ? Value : null
            };

            if (ListConditions.Any(x => x.address == rc.address))
            {
                if (MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                int index = ListConditions.FindIndex(x => x.address == rc.address);
                if (index == -1) return;
                ListConditions.RemoveAt(index);
            }

            ListConditions.Add(rc);
            ResetConditionFields();
            OnPropertyChanged("ListConditions");
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
                
                if (SelectedSensor.GetName() == "config")
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


    }
}
