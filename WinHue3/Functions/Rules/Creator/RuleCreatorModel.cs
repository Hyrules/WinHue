using System.ComponentModel.DataAnnotations;
using WinHue3.Utils;

namespace WinHue3.Functions.Rules.Creator
{
    public class RuleCreatorModel : ValidatableBindableBase
    {
        private string _ruleEnabled;
        private string _name;


        public RuleCreatorModel()
        {
            _ruleEnabled = "enabled";
            _name = string.Empty;
        }


        public string RuleEnabled
        {
            get => _ruleEnabled;
            set => SetProperty(ref _ruleEnabled,value);
        }

        [Required]
        [StringLength(32,MinimumLength = 1)]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }
    }
}
