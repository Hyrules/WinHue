using System.ComponentModel.DataAnnotations;
using WinHue3.ViewModels;

namespace WinHue3.Models
{
    public class RuleCreatorModel : ValidatableBindableBase
    {
        private string _ruleEnabled;
        private string _name;


        public RuleCreatorModel()
        {
            RuleEnabled = "enabled";
            Name = string.Empty;
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
