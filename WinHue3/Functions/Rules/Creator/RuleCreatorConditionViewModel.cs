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
        private ObservableCollection<TreeViewItem> _listConditionProperties;
        private string _conditionValue;
        private TreeViewItem _selectedConditionProperty;
        private string _conditionOperator;
        private ObservableCollection<RuleCondition> _listRuleConditions;
        private RuleCondition _selectedRuleCondition;

        public RuleCreatorConditionViewModel()
        {
            _listConditionHueObjects = new ObservableCollection<IHueObject>();
            _listRuleConditions = new ObservableCollection<RuleCondition>();
            _listConditionProperties = new ObservableCollection<TreeViewItem>();
        }

        public void Initialize(Bridge bridge,DataStore ds)
        {
            _bridge = bridge;
            _listConditionProperties.Add(TreeViewHelper.BuildPropertiesTreeFromDataStore(ds));

        }

        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand RemoveRuleConditionCommand => new RelayCommand(param => RemoveRuleCondition(), (param) => CanRemoveRuleCondition());
        public ICommand SelectRuleConditionCommand => new RelayCommand(param => SelectRuleCondition());
        public ICommand ClearSelectedRuleConditionCommand => new RelayCommand(param => ClearSelectedRuleCondition());

        private void ClearSelectedRuleCondition()
        {
            SelectedRuleCondition = null;
        }


        private void SelectRuleCondition()
        {
            if (_selectedRuleCondition == null) return;
            SelectedConditionProperty = ParseRuleForProperty(_selectedRuleCondition.address.ToString(), ListConditionProperties[0]);
            if (SelectedConditionProperty == null)
            {
                return;
            }
            ExpandAllNodes(ListConditionProperties[0], _selectedRuleCondition.address.ToString());
        }

        private void ExpandAllNodes(TreeViewItem treeItem,string path)
        {
            treeItem.IsExpanded = path.Contains(treeItem.Tag.ToString());
            foreach (var childItem in treeItem.Items.OfType<TreeViewItem>())
            {
                ExpandAllNodes(childItem,path);
            }
        }

        private TreeViewItem ParseRuleForProperty(string address, TreeViewItem rtvi)
        {
            if (rtvi.Tag.ToString().Equals(address)) return rtvi;
            if (rtvi.Items.Count == 0) return null;
            
            foreach (TreeViewItem r in rtvi.Items)
            {
                TreeViewItem nrt = ParseRuleForProperty(address, r);
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
            if (SelectedConditionProperty.Items.Count > 0) return false;
            if (!ConditionValue.IsValid() && ConditionOperator != "dx") return false;
            return true;
        }

        private void AddCondition()
        {
            DialogResult result = DialogResult.Yes;
            if (ListRuleConditions.Any(x => x.address.ToString().Equals(SelectedConditionProperty.Tag.ToString())))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleConditions.Remove(
                        ListRuleConditions.FirstOrDefault(x => x.address.ToString().Equals(SelectedConditionProperty.Tag.ToString())));
            }
            if(result == DialogResult.Yes)
                ListRuleConditions.Add(new RuleCondition(){address = new HueAddress(SelectedConditionProperty.Tag.ToString()), @operator = ConditionOperator, value = ConditionValue});
        }

        public ObservableCollection<TreeViewItem> ListConditionProperties
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

        public TreeViewItem SelectedConditionProperty
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
