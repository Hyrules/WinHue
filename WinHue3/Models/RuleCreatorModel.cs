using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Validation;

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
            get { return _ruleEnabled; }
            set { SetProperty(ref _ruleEnabled,value); }
        }

        [Required]
        [StringLength(20,MinimumLength = 1)]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name,value); }
        }
    }
}
