using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX
{
    public class Hsbk
    {
        private byte[] _hue;
        private byte[] _sat;
        private byte[] _bri;
        private byte[] _kelvin;

        public Hsbk()
        {
            _hue = new byte[2];
            _sat = new byte[2];
            _bri = new byte[2];
            _kelvin = new byte[2];
        }

        public ushort Length => Convert.ToUInt16(_hue.Length + _sat.Length + _bri.Length + _kelvin.Length);

        public ushort Hue => BitConverter.ToUInt16(_hue, 0);
        public ushort Sat => BitConverter.ToUInt16(_sat, 0);
        public ushort Bri => BitConverter.ToUInt16(_bri, 0);
        public ushort Kelvin => BitConverter.ToUInt16(_kelvin, 0);

        public Hsbk(ushort hue, ushort bri, ushort sat, ushort kelvin) 
        {
            _hue = new byte[2];
            _sat = new byte[2];
            _bri = new byte[2];
            _kelvin = new byte[2];

            BitConverter.GetBytes(hue).ToArray().CopyTo(_hue, 0);
            BitConverter.GetBytes(sat).ToArray().CopyTo(_sat, 0);
            BitConverter.GetBytes(bri).ToArray().CopyTo(_bri, 0);
            BitConverter.GetBytes(kelvin).ToArray().CopyTo(_kelvin, 0);
        }

        public Hsbk(byte[] bytes)
        {

        }

        public byte[] GetBytes()
        {
            byte[] result = new byte[_hue.Length + _sat.Length + _bri.Length + _kelvin.Length];
            _hue.CopyTo(result, 0);
            _sat.CopyTo(result, 2);
            _bri.CopyTo(result, 4);
            _kelvin.CopyTo(result, 6);
            return result;
        }

        public static implicit operator byte[](Hsbk hsbk)
        { 
            return hsbk.GetBytes();
        }
    }
}
