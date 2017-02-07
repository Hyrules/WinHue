using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models
{
    public class CloneRuleModel
    {
        public HueObject replacingobject { get; set; }
        public HueObject withobject { get; set; }

        public override string ToString()
        {
            return $"Replacing {replacingobject.Id} with {withobject.Id}";
        }
    }
}
