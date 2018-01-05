using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Responses
{
    public class Acknowledgement
    {
        private byte[] _source;
        private byte[] _sequence;

        public Acknowledgement(byte[] ack)
        {           
            Array.Copy(ack, 0, _source, 0, 4);
            Array.Copy(ack, 4, _source, 4, 1);
        }
        public string Source => Encoding.UTF8.GetString(_source.Reverse().ToArray());
        public ushort Sequence => BitConverter.ToUInt16(_sequence,0);
    }
}
