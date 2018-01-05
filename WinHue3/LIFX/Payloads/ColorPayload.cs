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
            _reserved = new byte[2];
            _hsbk = new Hsbk();   
            _duration = new byte[4];
        }

        public ColorPayload(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime)
        {
            _reserved = new byte[2];
            _duration = new byte[4];
            _hsbk = new Hsbk(color, bri, sat, kelvin);        
        }

        public ColorPayload(Hsbk hsbk, uint transitiontime)
        {
            _reserved = new byte[2];
            _duration = new byte[4];
            _hsbk = hsbk;
        }

        public new ushort Length =>       
           Convert.ToUInt16(_reserved.Length + _hsbk.Length + _duration.Length);
        
        private void SetBytes(ref byte[] array, uint value)
        {
            BitConverter.GetBytes(value).ToArray().CopyTo(array, 0);
        }

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[_reserved.Length + _hsbk.Length + _duration.Length];
            _reserved.CopyTo(bytes, 0); // 0 - 1
            _hsbk.GetBytes().CopyTo(bytes, 2);
            _duration.CopyTo(bytes, 10); // 10 - 13
            return bytes;
        }

        public static implicit operator byte[](ColorPayload payload)
        {
            return payload.GetBytes();
        }
    }
}
