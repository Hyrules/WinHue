using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using HueLib2.Objects.Rules;
using WinHue3.Utils;
using WinHue3.Validation;

namespace WinHue3.ViewModels
{
    public class RuleConditionViewModel : ValidatableBindableBase
    {
        private RuleAddress _address;
        private string _operator;
        private string _value;
        private bool _isvalid;
        private Type _propType;
        private DataStore _ds;
        private List<RuleTreeViewItem> _configProperties;
        private ObservableCollection<RuleTreeViewItem> _lightsProperties;
        private ObservableCollection<RuleTreeViewItem> _groupsProperties;
        private ObservableCollection<RuleTreeViewItem> _scenesProperties;
        private ObservableCollection<RuleTreeViewItem> _schedulesProperties;
        private ObservableCollection<RuleTreeViewItem> _resourceLinksProperties;
        private ObservableCollection<RuleTreeViewItem> _sensorsProperties;
        private ObservableCollection<RuleTreeViewItem> _ruleProperties;
        private RuleTreeViewItem _selectedProperty;

        public RuleConditionViewModel()
        {
            _configProperties = BuildTree(typeof(BridgeSettings), $"/config");
            _lightsProperties = new ObservableCollection<RuleTreeViewItem>();
            _groupsProperties = new ObservableCollection<RuleTreeViewItem>();
            _scenesProperties = new ObservableCollection<RuleTreeViewItem>();
            _schedulesProperties = new ObservableCollection<RuleTreeViewItem>();
            _resourceLinksProperties = new ObservableCollection<RuleTreeViewItem>();
            _sensorsProperties = new ObservableCollection<RuleTreeViewItem>();
            _ruleProperties = new ObservableCollection<RuleTreeViewItem>();
            _address = new RuleAddress();
            _operator = string.Empty;
            _isvalid = false;
            _operator = "eq";
        }

        public void Initialize(DataStore ds)
        {
            _ds = ds;
            foreach (KeyValuePair<string, Light> l in _ds.lights)
            {
                _lightsProperties.Add(new RuleTreeViewItem()
                {
                    Name = l.Value.Name,
                    Childrens = BuildTree(typeof(Light), $"/lights/{l.Key}"),
                    HObject = l.Value,
                    Path = $"/lights/{l.Key}",
                });
            }

            foreach (KeyValuePair<string, Group> g in _ds.groups)
            {
                _groupsProperties.Add(new RuleTreeViewItem()
                {
                    Name = g.Value.Name,
                    Childrens = BuildTree(typeof(Group), $"/groups/{g.Key}"),
                    HObject = g.Value,
                    Path = $"/groups/{g.Key}",
                });
            }

            foreach (KeyValuePair<string, Scene> s in _ds.scenes)
            {
                if (s.Value.Name.Contains("HIDDEN") && WinHueSettings.settings.ShowHiddenScenes == false)
                    continue;
                _scenesProperties.Add(new RuleTreeViewItem()
                {
                    Name = s.Value.Name,
                    Childrens = BuildTree(typeof(Scene), $"/scenes/{s.Key}"),
                    HObject = s.Value,
                    Path = $"/scenes/{s.Key}",
                });
            }

            foreach (KeyValuePair<string, Schedule> sc in _ds.schedules)
            {
                _schedulesProperties.Add(new RuleTreeViewItem()
                {
                    Name = sc.Value.Name,
                    Childrens = BuildTree(typeof(Schedule), $"/schedules/{sc.Key}"),
                    HObject = sc.Value,
                    Path = $"/schedules/{sc.Key}"
                });
            }

            foreach (KeyValuePair<string, Resourcelink> rl in _ds.resourcelinks)
            {
                _resourceLinksProperties.Add(new RuleTreeViewItem()
                {
                    Name = rl.Value.Name,
                    Childrens = BuildTree(typeof(Resourcelink), $"/resourcelinks/{rl.Key}"),
                    HObject = rl.Value,
                    Path = $"/resourcelinks/{rl.Key}"
                });
            }

            foreach (KeyValuePair<string, Sensor> se in _ds.sensors)
            {
                _sensorsProperties.Add(new RuleTreeViewItem()
                {
                    Name = se.Value.Name,
                    Childrens = BuildTree(se.Value.GetType(), $"/sensors/{se.Key}", se.Value),
                    HObject = se.Value,
                    Path = $"/sensors/{se.Key}"
                });
            }

            foreach (KeyValuePair<string, Rule> r in _ds.rules)
            {
                _ruleProperties.Add(new RuleTreeViewItem()
                {
                    Name = r.Value.Name,
                    Childrens = BuildTree(r.Value.GetType(), $"/rules/{r.Key}", r.Value),
                    HObject = r.Value,
                    Path = $"/rules/{r.Key}"
                });
            }
        }

        // THIS FUNCTION BUILD THE TREE DOWN FROM THE PROPERTIES OF A CLASS OR OBJECT
        private List<RuleTreeViewItem> BuildTree(Type type, string currentPath, object currentObject = null)
        {
            RuleTreeViewItem rtvi = new RuleTreeViewItem() { HObject = currentObject };
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
                        Childrens = BuildTree(t, $"{currentPath}/{pi.Name}", currentObject == null ? null : pi.GetValue(currentObject)),
                        Path = $"/{pi.Name}"
                    };
                }
                else
                {
                    rtvi = new RuleTreeViewItem() { Name = pi.Name, Property = pi, Childrens = null, Path = $"{currentPath}/{pi.Name}" };
                }
                lrtvi.Add(rtvi);
            }
            return lrtvi.OrderBy(x => x.Name).ToList();
        }

        public List<RuleTreeViewItem> ConfigProperties
        {
            get { return _configProperties; }
            set { SetProperty(ref _configProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> LightsProperties
        {
            get { return _lightsProperties; }
            set { SetProperty(ref _lightsProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> GroupsProperties
        {
            get { return _groupsProperties; }
            set { SetProperty(ref _groupsProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> ScenesProperties
        {
            get { return _scenesProperties; }
            set { SetProperty(ref _scenesProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> SchedulesProperties
        {
            get { return _schedulesProperties; }
            set { SetProperty(ref _schedulesProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> ResourceLinksProperties
        {
            get { return _resourceLinksProperties; }
            set { SetProperty(ref _resourceLinksProperties, value); }
        }

        public ObservableCollection<RuleTreeViewItem> SensorsProperties
        {
            get { return _sensorsProperties; }
            set { SetProperty(ref _sensorsProperties, value); }
        }

        public RuleTreeViewItem SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                SetProperty(ref _selectedProperty, value);
                Address = SelectedProperty == null ? null : new RuleAddress(SelectedProperty.Path);
                RaisePropertyChanged("Value");
            }
        }

        public ObservableCollection<RuleTreeViewItem> RuleProperties
        {
            get { return _ruleProperties; }
            set { SetProperty(ref _ruleProperties, value); }
        }

        public RuleAddress Address
        {
            get { return _address; }
            set { SetProperty(ref _address,value); }
        }

        public string Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator,value); }
        }

        [RuleValueValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Rule_NotProperFormat")]
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value,value); }
        }

        public bool IsValid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid,value); }
        }

        public Type PropType
        {
            get { return _selectedProperty?.Property.PropertyType; }
        
        }
/*
        public RuleCondition Condition
        {
            get
            {
                return new RuleCondition() { address = Address, @operator = Operator, value = Convert.ChangeType(_value, PropType) };
            }

            set
            {
                Address = value.address;
                switch (Address.objecttype)
                {
                    case "lights":
                        break;
                    case "groups":
                        break;
                    case 
                }
                SelectedProperty = 
            }

        }
        */
    }
}
