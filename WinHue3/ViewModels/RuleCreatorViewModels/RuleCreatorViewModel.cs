using System.Collections.ObjectModel;
using System.Linq;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using Bridge = WinHue3.Philips_Hue.BridgeObject.Bridge;

namespace WinHue3.ViewModels.RuleCreatorViewModels
{
    public class RuleCreatorViewModel: ValidatableBindableBase
    {
        private Bridge _bridge;
        private string _name;
        private bool _enabled;

        private RuleCreatorConditionViewModel _rccvm;
        private RuleCreatorActionViewModel _rcavm;

        public RuleCreatorViewModel()
        {
            Name = string.Empty;
            Enabled = true;
            RuleConditionViewModel = new RuleCreatorConditionViewModel();
            RuleActionViewModel = new RuleCreatorActionViewModel();
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
            _rccvm.SetBridge(bridge);
            _rcavm.SetBridge(bridge);
        }


        public RuleCreatorConditionViewModel RuleConditionViewModel
        {
            get => _rccvm;
            set => SetProperty(ref _rccvm, value);
        }

        public RuleCreatorActionViewModel RuleActionViewModel
        {
            get => _rcavm;
            set => SetProperty(ref _rcavm, value);
        }


        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public Rule Rule
        {
            get
            {
                Rule newrule = new Rule
                {
                    name = Name,
                    actions = RuleActionViewModel.ListRuleActions.ToList(),
                    conditions = RuleConditionViewModel.ListRuleConditions.ToList(),
                    status = Enabled ? "enabled" : "disabled"
                };
                return newrule;
            }
            set
            {
                Name = value.name;
                Enabled = value.status == "enabled";
                RuleConditionViewModel.ListRuleConditions = new ObservableCollection<RuleCondition>(value.conditions);
                RuleActionViewModel.ListRuleActions = new ObservableCollection<RuleAction>(value.actions);
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled,value);
        }
    }
}

