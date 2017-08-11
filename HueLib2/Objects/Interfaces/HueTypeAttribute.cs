using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2.Objects.Interfaces
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class HueType : Attribute
    {
        private string type;
        public HueType(string type)
        {
            this.type = type;
        }

        public string HueObjectType
        {
            get { return type; }
            set { type = value; }
        }
    }
}
