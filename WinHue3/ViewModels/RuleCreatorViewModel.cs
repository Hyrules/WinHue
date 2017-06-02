using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using HueLib2;
using HueLib2.Objects.Rules;
using WinHue3.Utils;
using MessageBox = System.Windows.Forms.MessageBox;
using Rule = HueLib2.Rule;

namespace WinHue3.ViewModels
{
    public class RuleCreatorViewModel : ValidatableBindableBase
    {
        private DataStore _ds;
        private List<RuleTreeViewItem> _configProperties;
        private ObservableCollection<RuleTreeViewItem> _lightsProperties;
        private ObservableCollection<RuleTreeViewItem> _groupsProperties;
        private ObservableCollection<RuleTreeViewItem> _scenesProperties;
        private ObservableCollection<RuleTreeViewItem> _schedulesProperties;
        private ObservableCollection<RuleTreeViewItem> _resourceLinksProperties;
        private ObservableCollection<RuleTreeViewItem> _sensorsProperties;
        private ObservableCollection<RuleTreeViewItem> _ruleProperties;
        private ObservableCollection<RuleCondition> _listConditions;
        private ObservableCollection<RuleTreeViewItem> _listActions;

        private RuleTreeViewItem _selectedProperty;

        public RuleCreatorViewModel()
        {
            _configProperties = BuildTree(typeof(BridgeSettings),$"/config");
            _lightsProperties = new ObservableCollection<RuleTreeViewItem>();
            _groupsProperties = new ObservableCollection<RuleTreeViewItem>();
            _scenesProperties = new ObservableCollection<RuleTreeViewItem>();    
            _schedulesProperties = new ObservableCollection<RuleTreeViewItem>();
            _resourceLinksProperties = new ObservableCollection<RuleTreeViewItem>();
            _sensorsProperties = new ObservableCollection<RuleTreeViewItem>();
            _ruleProperties = new ObservableCollection<RuleTreeViewItem>();
            _listActions = new ObservableCollection<RuleTreeViewItem>();
            _listConditions = new ObservableCollection<RuleCondition>();

        }

        public List<RuleTreeViewItem> ConfigProperties
        {
            get { return _configProperties; }
            set { SetProperty(ref _configProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> LightsProperties
        {
            get { return _lightsProperties; }
            set { SetProperty(ref _lightsProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> GroupsProperties
        {
            get { return _groupsProperties; }
            set { SetProperty(ref _groupsProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> ScenesProperties
        {
            get { return _scenesProperties; }
            set { SetProperty(ref _scenesProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> SchedulesProperties
        {
            get { return _schedulesProperties; }
            set { SetProperty(ref _schedulesProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> ResourceLinksProperties
        {
            get { return _resourceLinksProperties; }
            set { SetProperty(ref _resourceLinksProperties,value); }
        }

        public ObservableCollection<RuleTreeViewItem> SensorsProperties
        {
            get { return _sensorsProperties; }
            set { SetProperty(ref _sensorsProperties, value); }
        }

        public RuleTreeViewItem SelectedProperty
        {
            get { return _selectedProperty; }
            set { SetProperty(ref _selectedProperty,value); }
        }

        public ObservableCollection<RuleTreeViewItem> RuleProperties
        {
            get { return _ruleProperties; }
            set { SetProperty(ref _ruleProperties,value); }
        }

        public ObservableCollection<RuleCondition> ListConditions
        {
            get { return _listConditions; }
            set { SetProperty(ref _listConditions, value); }
        }

        public ObservableCollection<RuleTreeViewItem> ListActions
        {
            get { return _listActions; }
            set { SetProperty(ref _listActions, value); }
        }

        public void Initialize(DataStore currentDataStore)
        {
            _ds = currentDataStore;

            foreach (KeyValuePair<string, Light> l in _ds.lights)
            {
                _lightsProperties.Add(new RuleTreeViewItem()
                {
                    Name = l.Value.name,
                    Childrens = BuildTree(typeof(Light),$"/lights/{l.Key}"),
                    HObject = l.Value,
                    Path= $"/lights/{l.Key}",
                });
            }

            foreach (KeyValuePair<string, Group> g in _ds.groups)
            {
                _groupsProperties.Add(new RuleTreeViewItem()
                {
                    Name = g.Value.name,
                    Childrens = BuildTree(typeof(Group),$"/groups/{g.Key}"),
                    HObject = g.Value,
                    Path = $"/groups/{g.Key}",
                });
            }

            foreach (KeyValuePair<string, Scene> s in _ds.scenes)
            {
                if (s.Value.name.Contains("HIDDEN") && WinHueSettings.settings.ShowHiddenScenes == false)
                    continue;
                _scenesProperties.Add(new RuleTreeViewItem()
                {
                    Name = s.Value.name,
                    Childrens = BuildTree(typeof(Scene),$"/scenes/{s.Key}"),
                    HObject = s.Value,
                    Path = $"/scenes/{s.Key}",
                });
            }

            foreach (KeyValuePair<string, Schedule> sc in _ds.schedules)
            {
                _schedulesProperties.Add(new RuleTreeViewItem()
                {
                    Name = sc.Value.name,
                    Childrens = BuildTree(typeof(Schedule),$"/schedules/{sc.Key}"),
                    HObject = sc.Value,
                    Path= $"/schedules/{sc.Key}"
                });
            }

            foreach (KeyValuePair<string, Resourcelink> rl in _ds.resourcelinks)
            {
                _resourceLinksProperties.Add(new RuleTreeViewItem()
                {
                    Name = rl.Value.name,
                    Childrens = BuildTree(typeof(Resourcelink),$"/resourcelinks/{rl.Key}"),
                    HObject = rl.Value,
                    Path= $"/resourcelinks/{rl.Key}"
                });
            }

            foreach (KeyValuePair<string, Sensor> se in _ds.sensors)
            {
                _sensorsProperties.Add(new RuleTreeViewItem()
                {
                    Name = se.Value.name,
                    Childrens = BuildTree(se.Value.GetType(),$"/sensors/{se.Key}", se.Value),
                    HObject = se.Value,
                    Path= $"/sensors/{se.Key}"
                });
            }

            foreach (KeyValuePair<string, Rule> r in _ds.rules)
            {
                _ruleProperties.Add(new RuleTreeViewItem()
                {
                    Name = r.Value.name,
                    Childrens = BuildTree(r.Value.GetType(),$"/rules/{r.Key}" ,r.Value),
                    HObject = r.Value,
                    Path= $"/rules/{r.Key}"
                });
            }
        }

        // THIS FUNCTION BUILD THE TREE DOWN FROM THE PROPERTIES OF A CLASS OR OBJECT
        private List<RuleTreeViewItem> BuildTree(Type type,string currentPath, object currentObject = null)
        {
            RuleTreeViewItem rtvi = new RuleTreeViewItem(){ HObject = currentObject};
            List<RuleTreeViewItem> lrtvi = new List<RuleTreeViewItem>();
            PropertyInfo[] prop = type.GetProperties();
         
            foreach (PropertyInfo pi in prop)
            {
                Type t = pi.PropertyType;

                if (currentObject != null)
                {
                    t = pi.GetValue(currentObject)?.GetType();
                    
                }
                    
                if (pi.PropertyType.Assembly == type.Assembly)
                {
                   
                    rtvi = new RuleTreeViewItem()
                    {
                        Name = pi.Name,
                        Property = pi,
                        Childrens = BuildTree(t,$"{currentPath}/{pi.Name}",currentObject == null ? null : pi.GetValue(currentObject)),
                        Path=$"/{pi.Name}"
                    };
                }
                else
                {
                    rtvi = new RuleTreeViewItem() {Name = pi.Name, Property = pi, Childrens = null, Path = $"{currentPath}/{pi.Name}"};
                }
                lrtvi.Add(rtvi);
            }
            return lrtvi.OrderBy(x => x.Name).ToList();
        }

        private bool CanAddAction()
        {
            if (SelectedProperty == null) return false;
            return true;
        }


        private void AddAction()
        {
            RuleAction ra = new RuleAction()
            {
                address = new RuleAddress(SelectedProperty.Path),
                method = "PUT",

            };


            SelectedProperty = null;
        }

        private bool CanAddCondition()
        {
            if (SelectedProperty == null) return false;
          //  if (!ConditionValue.IsValid()) return false;
            return true;
        }

        private void AddCondition()
        {

            RuleCondition rc = new RuleCondition
            {
                address = new RuleAddress(SelectedProperty.Path),
            };


            if (!ListConditions.Any(x => x.address.ToString() == SelectedProperty.Path))
            {
                ListConditions.Add(rc);
                SelectedProperty = null;
            }
            else
            {
                if (MessageBox.Show(GlobalStrings.Rule_ConditionAlreadyExists, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ListConditions.Remove(ListConditions.First(x => x.ToString() == SelectedProperty.Path));
                    ListConditions.Add(rc);
                    SelectedProperty = null;

                }

            }
        }

        public ICommand AddActionCommand => new RelayCommand(param => AddAction(),(param)=> CanAddAction());
        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param)=> CanAddCondition());


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
                System.Windows.MessageBox.Show(GlobalStrings.Rule_NotProperFormat + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException)
            {
                result = null;
                System.Windows.MessageBox.Show(GlobalStrings.Rule_ValueNotInRange + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
