using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Payloads
{
    public abstract class Payload
    {
        public abstract int Length { get; }
        public abstract byte[] GetBytes();

        public static implicit operator byte[](Payload payload)
        {
            return payload.GetBytes();
        }
    }
}
