using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Responses
{
    public class State
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

        public State(byte[] bytes)
        {
            _hsbk = new Hsbk(bytes.Take(_hsbk.Length).ToArray());
            Array.Copy(bytes, 8, _power, 0, _power.Length);
            Array.Copy(bytes, 10, _label, 0, _label.Length);
        }

        public Hsbk HSBK => _hsbk;
        public ushort Power => BitConverter.ToUInt16(_power,0);
        public string Label => Encoding.UTF8.GetString(_label.Reverse().ToArray());

    }
}
