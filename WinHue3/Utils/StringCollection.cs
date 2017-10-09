using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Utils
{
    [Serializable]
    public class StringCollection : List<string>
    {
        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}
