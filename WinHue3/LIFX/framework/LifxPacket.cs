using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX.Responses.States.Device;

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

        public ushort Length
        {
            get
            {
                ushort len = _header.Length;
                if (_payload != null)
                    len = Convert.ToUInt16(len + _payload.Length);
                return len;
            }
        }

        public LifxPacket()
        {
            _header = new Header();

        }

        public LifxPacket(byte[] bytes)
        {
            _header = new Header(bytes.Take(36).ToArray());
            byte[] payload = bytes.Skip(36).Take(bytes.Length - 36).ToArray();

            switch (_header.MsgType)
            {
                case Header.MessageType.Light_State:
                    _payload = new State(payload);
                    break;
                case Header.MessageType.Light_StatePower:
                    _payload = new StatePower(payload);
                    break;
                case Header.MessageType.Device_StateInfo:
                    _payload = new StateInfo(payload);
                    break;
                case Header.MessageType.Device_StateGroup:
                    _payload = new StateGroup(payload);
                    break;
                case Header.MessageType.Device_StateVersion:
                    _payload = new StateVersion(payload);
                    break;
                case Header.MessageType.Device_StateLabel:
                    _payload = new StateLabel(payload);
                    break;
                case Header.MessageType.Device_StateHostInfo:
                    _payload = new StateHostInfo(payload);
                    break;
                case Header.MessageType.Device_StateLocation:
                    _payload = new StateLocation(payload);
                    break;
                case Header.MessageType.Device_StateHostFirmware:
                    _payload = new StateHostFirmware(_payload);
                    break;
                case Header.MessageType.Device_GetWifiInfo:
                    _payload = new StateHostInfo(_payload);
                    break;
                default:
                    break;
            }
        }

        public static implicit operator byte[] (LifxPacket packet)
        {          
            return packet.GetBytes();
        }


        private byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _header.GetBytes().CopyTo(bytes,0);
            _payload?.GetBytes().CopyTo(bytes,_header.Length);
            return bytes;
        }
    }
}
