using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Light
{
    public class WaveformPayload : Payload
    {
        byte[] _reserved;
        byte[] _transient;
        byte[] _hsbk;

        public WaveformPayload()
        {
            _reserved = new byte[1];
            _transient = new byte[1];
        }

        public override int Length => throw new NotImplementedException();

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[];
            return bytes;
        }
    }
}
