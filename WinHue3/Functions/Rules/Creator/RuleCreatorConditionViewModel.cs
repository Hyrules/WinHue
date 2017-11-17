using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Rules.Validation;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Rules.Creator
{
    public class RuleCreatorConditionViewModel : ValidatableBindableBase
    {
        private Bridge _bridge;
        private ObservableCollection<IHueObject> _listConditionHueObjects;
        private ObservableCollection<HuePropertyTreeViewItem> _listConditionProperties;
        private string _conditionValue;
        private HuePropertyTreeViewItem _selectedConditionProperty;
        private string _conditionOperator;
        private ObservableCollection<RuleCondition> _listRuleConditions;
        private RuleCondition _selectedRuleCondition;

        public RuleCreatorConditionViewModel()
        {
            _listConditionHueObjects = new ObservableCollection<IHueObject>();
            _listConditionProperties = new ObservableCollection<HuePropertyTreeViewItem>();
            _listRuleConditions = new ObservableCollection<RuleCondition>();
        }

        public void Initialize()
        {
            DataStore ds = _bridge.GetBridgeDataStore();

            HuePropertyTreeViewItem tvi = TreeViewHelper.BuildPropertiesTreeFromDataStore(ds);

            foreach (HuePropertyTreeViewItem t in tvi.Childrens)
            {
                _listConditionProperties.Add(t);    
            }

            
        }

        public void SetBridge(Bridge bridge)
        {
            _bridge = bridge;
        }


        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand RemoveRuleConditionCommand => new RelayCommand(param => RemoveRuleCondition(), (param) => CanRemoveRuleCondition());
        public ICommand SelectRuleConditionCommand => new RelayCommand(param => SelectRuleCondition());
        public ICommand ClearSelectedRuleConditionCommand => new RelayCommand(param => ClearSelectedRuleCondition());
        public ICommand ChangeSelectedConditionPropertyCommand => new RelayCommand(param => ChangeSelectedConditionProperty((HuePropertyTreeViewItem)param));

        private void ChangeSelectedConditionProperty(HuePropertyTreeViewItem tvi)
        {
            SelectedConditionProperty = tvi;
        }

        private void ClearSelectedRuleCondition()
        {
            SelectedRuleCondition = null;
        }


        private void SelectRuleCondition()
        {
         /*   if (SelectedRuleCondition == null) return;
            ObjectType = SelectedRuleCondition.address.objecttype;
            if (ObjectType != "config")
            {
                SelectedConditionHueObject = ListConditionHueObjects.FirstOrDefault(x => x.Id == SelectedRuleCondition.address.id);
                if (SelectedConditionHueObject != null)
                {
                    PopulateConditionProperties(SelectedRuleCondition.address.ToString());
                }
            }

            foreach(HuePropertyTreeViewItem r in ListConditionProperties)
            {
               SelectedConditionProperty = ParseRuleForProperty(SelectedRuleCondition.address.ToString(), r);
                if (SelectedConditionProperty != null) break;
            }

            ConditionOperator = SelectedRuleCondition.@operator;
            ConditionValue = SelectedRuleCondition.value;*/
        }

        private HuePropertyTreeViewItem ParseRuleForProperty(string address, HuePropertyTreeViewItem rtvi)
        {
            if (rtvi.Name.Equals(address)) return rtvi;
            if (rtvi.Childrens.Count == 0) return null;
            
            foreach (HuePropertyTreeViewItem r in rtvi.Childrens)
            {
                HuePropertyTreeViewItem nrt = ParseRuleForProperty(address, r);
                if (nrt != null) return nrt;
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
            if (!ConditionValue.IsValid() && ConditionOperator != "dx") return false;
            return true;
        }

        private void AddCondition()
        {
            DialogResult result = DialogResult.Yes;
            if (ListRuleConditions.Any(x => x.address.ToString().Equals(SelectedConditionProperty.Name)))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleConditions.Remove(
                        ListRuleConditions.FirstOrDefault(x => x.address.ToString().Equals(SelectedConditionProperty.Name)));
            }
            if(result == DialogResult.Yes)
                ListRuleConditions.Add(new RuleCondition(){address = new HueAddress(SelectedConditionProperty.Name), @operator = ConditionOperator, value = ConditionValue});
        }

        public ObservableCollection<HuePropertyTreeViewItem> ListConditionProperties
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
            set => SetProperty(ref _listRuleConditions,value);
        }

        public RuleCondition SelectedRuleCondition
        {
            get => _selectedRuleCondition;
            set => SetProperty(ref _selectedRuleCondition,value);
        }


    }
}
