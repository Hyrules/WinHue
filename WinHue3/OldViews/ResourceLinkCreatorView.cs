using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3
{
    public class ResourceLinkCreatorView : View
    {
        private Resourcelink rl;


        public ResourceLinkCreatorView()
        {
            rl = new Resourcelink();
        }

        public string Name
        {
            get { return rl.name; }
            set
            {
                rl.name = value;
                OnPropertyChanged();
            }
        }


    }
}
