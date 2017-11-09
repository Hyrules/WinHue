using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Utils;
using WinHue3.Validation;


namespace WinHue3.ViewModels.RuleCreatorViewModels
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
            ListConditionHueObjects = new ObservableCollection<IHueObject>();
            ListConditionProperties = new ObservableCollection<TreeViewItem>();
            ListRuleConditions = new ObservableCollection<RuleCondition>();
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
                SelectedConditionHueObject =
                    ListConditionHueObjects.FirstOrDefault(x => x.Id == SelectedRuleCondition.address.id);
                if(SelectedConditionHueObject != null)
                    PopulateConditionProperties(SelectedRuleCondition.address.ToString());
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
            List<TreeViewItem> lrtvi = new List<TreeViewItem>();

            PropertyInfo[] props = SelectedConditionHueObject?.GetType().GetHueProperties();
            if (props == null && ObjectType == "config") props = new BridgeSettings().GetType().GetProperties();
            

            foreach (PropertyInfo pi in props)
            {
                lrtvi.Add(BuildTree(pi, path, selectedpath));
                

            }

            ListConditionProperties.AddRange(lrtvi);

        }

        private static TreeViewItem BuildTree(PropertyInfo root, string currentpath, string selectedpath = null)
        {
            var toVisit = new Stack<PropertyInfo>();
            var toVisitRtvi = new Stack<TreeViewItem>();
            var visitedAncestors = new Stack<PropertyInfo>();
            var pathstack = new Stack<string>();
            var actualpath = currentpath + "/" + root.Name;
            var rtvi = new TreeViewItem() { Header = root.Name, Tag = actualpath };

            toVisit.Push(root);
            while (toVisit.Count > 0)
            {

                var node = toVisit.Peek();
                if (node.PropertyType.GetHueProperties().Length > 0)
                {


                    if (visitedAncestors.PeekOrDefault() != node)
                    {
                        visitedAncestors.Push(node);
                        toVisit.PushReverse(node.PropertyType.GetHueProperties().ToList());

                        if (root.Name != node.Name)
                        {
                            pathstack.Push(actualpath);
                            actualpath = actualpath + "/" + node.Name;
                            toVisitRtvi.Push(rtvi);
                            rtvi = new TreeViewItem() { Header = node.Name, Tag = actualpath };
                        }
                        continue;
                    }
                    visitedAncestors.Pop();
                    if (toVisitRtvi.Count == 0) return rtvi;
                    TreeViewItem currtvi = toVisitRtvi.Pop();
                    currtvi.Items.Add(rtvi);
                    actualpath = pathstack.Pop();
                    currtvi.Tag = actualpath;
                    rtvi = currtvi;
                    toVisit.Pop();
                    continue;
                }


                if (visitedAncestors.Count > 0)
                {
                    TreeViewItem nrtvi = new TreeViewItem() { Header = node.Name,  Tag = actualpath + "/" + node.Name};
                    if (selectedpath != null && nrtvi.Tag.ToString().Equals(selectedpath)) nrtvi.IsSelected = true;
                    rtvi.Items.Add(nrtvi);
                    rtvi.IsExpanded = true;
                    if(nrtvi.IsSelected)
                        nrtvi.BringIntoView();
                }

                toVisit.Pop();
            }

            return rtvi;
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
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeLights(_bridge));
                    break;
                case "groups":
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeGroups(_bridge));
                    break;
                case "rules":
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeRules(_bridge));
                    break;
                case "resourcelinks":
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeResourceLinks(_bridge));
                    break;
                case "scenes":
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeScenes(_bridge));
                    break;
                case "config":
                    PopulateConditionProperties();
                    break;
                case "sensors":
                    CollectionExtensions.AddRange(ListConditionHueObjects, HueObjectHelper.GetBridgeSensors(_bridge));
                    break;
                default:
                    break;
            }


        }

    }
}
