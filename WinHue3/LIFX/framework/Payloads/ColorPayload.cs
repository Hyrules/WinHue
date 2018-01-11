using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Payloads
{
    //*** PAYLOAD *** HSBK
    public class ColorPayload : Payload
    {
        private byte[] _reserved;
        private Hsbk _hsbk;
        private byte[] _duration;

        public ColorPayload()
        {
            _reserved = new byte[1];
            _hsbk = new Hsbk();   
            _duration = new byte[4];
        }

        public ColorPayload(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime) : this()
        {
            _hsbk = new Hsbk(color, bri, sat, kelvin);
            _duration = BitConverter.GetBytes(transitiontime);
        }

        public ColorPayload(Hsbk hsbk, uint transitiontime) : this()
        {
            _hsbk = hsbk;
            _duration = BitConverter.GetBytes(transitiontime);
        }

        public override int Length => _reserved.Length + _hsbk.Length + _duration.Length;
        
        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _reserved.CopyTo(bytes, 0); // 0 
            _hsbk.GetBytes().CopyTo(bytes, 1); // 1 - 8
            _duration.CopyTo(bytes, 9); // 9 - 12
            return bytes;
        }

        public static implicit operator byte[](ColorPayload payload)
        {
            return payload.GetBytes();
        }
    }
}
