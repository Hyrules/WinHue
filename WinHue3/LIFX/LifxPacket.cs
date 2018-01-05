using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX
{
    public class LifxPacket
    {
        // LIFX USES LITTLE ENDIAN THIS MEANS THAT THE MOST SIGNIFICAT BIT IS REVERSE (least significant byte is stored first)

        private Header _header;
        private Payload _payload;

        public Header Header { get => _header; private set => _header = value; }
        public Payload Payload
        {
            get => _payload;
            set
            {
                _payload = value;
                _header.SetSize(Convert.ToUInt16(_header.Length + _payload.Length));
            }

        }

        public LifxPacket()
        {
            _header = new Header();

        }

        public static implicit operator byte[] (LifxPacket packet)
        {          
            return packet.GetBytes();
        }


        private byte[] GetBytes()
        {
            return new byte[2];
        }
    }
}
