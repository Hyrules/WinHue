using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Rules.Validation;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Rules.Creator
{
    public class RuleCreatorConditionViewModel : ValidatableBindableBase
    {
        private Bridge _bridge;
        private ObservableCollection<IHueObject> _listConditionHueObjects;
        private string _objectType;
        private IHueObject _selectedConditionHueObject;
        private ObservableCollection<TreeViewItem> _listConditionProperties;
        private string _conditionValue;
        private TreeViewItem _selectedConditionProperty;
        private string _conditionOperator;
        private ObservableCollection<RuleCondition> _listRuleConditions;
        private RuleCondition _selectedRuleCondition;

        public RuleCreatorConditionViewModel()
        {
            _listConditionHueObjects = new ObservableCollection<IHueObject>();
            _listConditionProperties = new ObservableCollection<TreeViewItem>();
            _listRuleConditions = new ObservableCollection<RuleCondition>();
        }

        public void SetBridge(Bridge bridge)
        {
            _bridge = bridge;
        }

        public ObservableCollection<IHueObject> ListConditionHueObjects
        {
            get => _listConditionHueObjects;
            set => SetProperty(ref _listConditionHueObjects, value);
        }

        public string ObjectType
        {
            get => _objectType;
            set => SetProperty(ref _objectType, value);
        }

        public ICommand PopulateConditionObjectsCommand => new RelayCommand(param => PopulateObjects());
        public ICommand PopulateConditionPropertiesCommand => new RelayCommand(param => PopulateConditionProperties());
        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param) => CanAddCondition());
        public ICommand RemoveRuleConditionCommand => new RelayCommand(param => RemoveRuleCondition(), (param) => CanRemoveRuleCondition());
        public ICommand SelectRuleConditionCommand => new RelayCommand(param => SelectRuleCondition());
        public ICommand ClearSelectedRuleConditionCommand => new RelayCommand(param => ClearSelectedRuleCondition());
        public ICommand ChangeSelectedConditionPropertyCommand => new RelayCommand(param => ChangeSelectedConditionProperty((TreeViewItem)param));

        private void ChangeSelectedConditionProperty(TreeViewItem tvi)
        {
            SelectedConditionProperty = tvi;
        }

        private void ClearSelectedRuleCondition()
        {
            SelectedRuleCondition = null;
        }


        private void SelectRuleCondition()
        {
            if (SelectedRuleCondition == null) return;
            ObjectType = SelectedRuleCondition.address.objecttype;
            if (ObjectType != "config")
            {
                SelectedConditionHueObject = ListConditionHueObjects.FirstOrDefault(x => x.Id == SelectedRuleCondition.address.id);
                if (SelectedConditionHueObject != null)
                {
                    PopulateConditionProperties(SelectedRuleCondition.address.ToString());
                }
            }

            foreach(TreeViewItem r in ListConditionProperties)
            {
               SelectedConditionProperty = ParseRuleForProperty(SelectedRuleCondition.address.ToString(), r);
                if (SelectedConditionProperty != null) break;
            }

            ConditionOperator = SelectedRuleCondition.@operator;
            ConditionValue = SelectedRuleCondition.value;
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
            if (ObjectType != "config")
            {
                if (SelectedConditionHueObject == null) return false;
            }
            if (!ConditionValue.IsValid() && ConditionOperator != "dx") return false;
            return true;
        }

        private void AddCondition()
        {
            DialogResult result = DialogResult.Yes;
            if (ListRuleConditions.Any(x => x.address.ToString().Equals(SelectedConditionProperty.Tag)))
            {
                result = MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ListRuleConditions.Remove(
                        ListRuleConditions.FirstOrDefault(x => x.address.ToString().Equals(SelectedConditionProperty.Tag)));
            }
            if(result == DialogResult.Yes)
                ListRuleConditions.Add(new RuleCondition(){address = new HueAddress(SelectedConditionProperty.Tag.ToString()), @operator = ConditionOperator, value = ConditionValue});
        }

        public IHueObject SelectedConditionHueObject
        {
            get => _selectedConditionHueObject;
            set => SetProperty(ref _selectedConditionHueObject, value);
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

        private void PopulateConditionProperties(string selectedpath = null)
        {
            ListConditionProperties.Clear();
            if (ObjectType == null || SelectedConditionHueObject == null ) return;
            string path = SelectedConditionHueObject == null ? $"/config" : $"/{SelectedConditionHueObject.GetHueType()}/{SelectedConditionHueObject.Id}";

            TreeViewItem tvi = TreeViewHelper.BuildPropertiesTree(SelectedConditionHueObject, path, selectedpath);

            List<TreeViewItem> lrtvi = tvi.Items.Cast<TreeViewItem>().ToList();

            ListConditionProperties.AddRange(lrtvi);

        }

        private void PopulateObjects()
        {
            ListConditionHueObjects.Clear();
            ListConditionProperties.Clear();
            SelectedConditionProperty = null;
            SelectedConditionHueObject = null;

            switch (ObjectType)
            {
                case "lights":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeLights(_bridge));
                    break;
                case "groups":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeGroups(_bridge));
                    break;
                case "rules":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeRules(_bridge));
                    break;
                case "resourcelinks":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeResourceLinks(_bridge));
                    break;
                case "scenes":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeScenes(_bridge));
                    break;
                case "config":
                    PopulateConditionProperties();
                    break;
                case "sensors":
                    ListConditionHueObjects.AddRange(HueObjectHelper.GetBridgeSensors(_bridge));
                    break;
                default:
                    break;
            }


        }

    }
}
