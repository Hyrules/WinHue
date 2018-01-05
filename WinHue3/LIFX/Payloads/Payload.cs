using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Payloads
{
    public abstract class Payload
    {
        public ushort Length;
        public abstract byte[] GetBytes();
        
    }
}
