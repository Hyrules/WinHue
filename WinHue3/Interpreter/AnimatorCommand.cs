using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib_base;

namespace WinHue3
{
    public abstract class AnimatorCommand
    {
        public string name;
        public string comment;
    }

    public class AnimatorCommandLoop : AnimatorCommand
    {
        public int amount;
        public int loopleft;
    }

    public class AnimatorCommandEndLoop : AnimatorCommand
    {
        public int gotobackline;
    }

    public class AnimatorCommandWait : AnimatorCommand
    {
        public int waitamount;
    }

    public class AnimatorCommandSet : AnimatorCommand
    {
        public HueObject objset;
    }
}
