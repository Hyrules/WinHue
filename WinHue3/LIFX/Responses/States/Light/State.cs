using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses
{
    public class State : Payload
    {
        private Hsbk _hsbk;
        private byte[] _reserved;
        private byte[] _power;
        private byte[] _label;
        private byte[] _reserved2;

        public State()
        {
            _hsbk = new Hsbk(); // [8]
            _reserved = new byte[2];
            _power = new byte[2];
            _label = new byte[32];
            _reserved2 = new byte[8];
        }

        public State(byte[] bytes) : this()
        {
            _hsbk = new Hsbk(bytes.Take(8).ToArray()); // 0 - 7
            Array.Copy(bytes, 8,_reserved, 0, _reserved.Length); // 8- 9
            Array.Copy(bytes, 10, _power, 0, _power.Length); // 10 - 11
            Array.Copy(bytes, 12, _label, 0, _label.Length); // 12 - 43
            Array.Copy(bytes, 44,_reserved2,0,_reserved2.Length); // 44 - 51
        }

        public Hsbk HSBK => _hsbk;
        public ushort Power => BitConverter.ToUInt16(_power,0);
        public string Label => Encoding.UTF8.GetString(_label).TrimEnd('\0');

        public override int Length => _hsbk.Length + _reserved.Length + _power.Length + _label.Length + _reserved2.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _hsbk.GetBytes().CopyTo(bytes,0);
            _reserved.CopyTo(bytes,8);
            _power.CopyTo(bytes,10);
            _label.CopyTo(bytes,12);
            _reserved2.CopyTo(bytes,44);
            return bytes;
        }
    }
}
