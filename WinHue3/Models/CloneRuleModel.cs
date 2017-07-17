using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2.Objects.HueObject;

namespace WinHue3.Models
{
    public class CloneRuleModel
    {
        public IHueObject replacingobject { get; set; }
        public IHueObject withobject { get; set; }

        public override string ToString()
        {
            return $"Replacing {replacingobject.Id} with {withobject.Id}";
        }
    }
}
