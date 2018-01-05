using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX
{
    public class Header
    {
        public enum MessageType
        {
            Device_GetService = 2,
            Device_StateService = 3,
            Device_GetHostInfo = 12,
            Device_StateHostInfo = 13,
            Device_GetHostFirmware = 14,
            Device_StateHostFirmware = 15,
            Device_GetWifiInfo = 16,
            Device_StateWifiInfo = 17,
            Device_GetWifiFirmware = 18,
            Device_StateWifiFirmware = 19,
            Device_GetPower = 20,
            Device_SetPower = 21,
            Device_StatePower = 22,
            Device_GetLabel = 23,
            Device_SetLabel = 24,
            Device_StateLabel = 25,
            Device_GetVerion = 32,
            Device_StateVersion = 33,
            Device_GetInfo = 34,
            Device_StateInfo = 35,
            Device_Acknowledgement = 45,
            Device_GetLocation = 48,
            Device_SetLocation = 49,
            Device_StateLocation = 50,
            Device_GetGroup = 51,
            Device_SetGroup = 52,
            Device_StateGroup = 53,
            Device_EchoRequest = 58,
            Device_EchoResponse = 59,
            Light_Get = 101,
            Light_SetColor = 102,
            Light_SetWaveForm = 103,
            Light_SetWaveFormOptional = 119,
            Light_State = 107,
            Light_GetPower = 116,
            Light_SetPower = 117,
            Light_StatePower = 118,
            Light_GetInfrared = 120,
            Light_StateInfrared = 121,
            Light_SetInfrared = 122,
            MultiZone_SetColorZones = 501,
            MultiZone_GetcolorZones = 502,
            MultiZone_StateZones = 503,
            MultiZone_StateMultiZone = 206
        }

        //*** FRAME HEADER ***
        private byte[] _size;
        private byte[] _header;
        private byte[] _uniqueid;

        //*** FRAME ADDRESS ***
        private byte[] _target;
        private byte[] _reservedfield1;
        private byte[] _ack;
        private byte[] _sequence;
        private byte[] _protocolheader;
        private byte[] _reservedfield2;
        private byte[] _msgtype;

        public Header()
        {
            _size = new byte[2];
            _header = new byte[2] { 0x00, 0x34 };
            _uniqueid = new byte[4] { 0x00, 0x03, 0x48, 0x56 }; // WH30 (reversed)
            _target = new byte[8]; // MAC ADDRESS OF THE DEVICE OR ALL ZERO FOR ALL 

            _reservedfield1 = new byte[6]; // MUST BE ZERO
            _ack = new byte[1] { 0x03 };
            _sequence = new byte[1] { 0x00 };
            _protocolheader = new byte[8]; // MUST BE ZERO
            _msgtype = new byte[2];
            _reservedfield2 = new byte[2]; // MUST BE ZERO

        }

        public ushort Length => 
            Convert.ToUInt16(_size.Length + _header.Length + _uniqueid.Length + _target.Length + 
                _reservedfield1.Length + _ack.Length + _sequence.Length + _protocolheader.Length + _msgtype.Length + _reservedfield2.Length);

        public IPAddress Target => new IPAddress(_target.Reverse().ToArray());
        public MessageType MsgType => (MessageType)BitConverter.ToUInt16(_msgtype, 0);

        public void SetSize(ushort size)
        {
            BitConverter.GetBytes(size).ToArray().CopyTo(_size, 0);
        }

        public byte[] SetTargetIP(IPAddress ip)
        {
            _target = ip.GetAddressBytes().Reverse().ToArray();
            return _target;
        }

        public void SetMessageType(MessageType msgtype)
        {
            BitConverter.GetBytes((ushort)msgtype).ToArray().CopyTo(_msgtype, 0);
        }

        private byte[] GetHeader()
        {
            byte[] header = new byte[36];

            //**** FRAME HEADER
            // SIZE is 0 - 1 and is the size of the whole message (including size)

            _header.CopyTo(header, 2); // 2 - 3
            _uniqueid.CopyTo(header, 4); // 4 - 8

            //**** FRAME ADDRESS
            _target.CopyTo(header, 9); // 9 - 16
            _reservedfield1.CopyTo(header, 17); // 17 - 22 
            _ack.CopyTo(header, 23);
            _sequence.CopyTo(header, 24);
            _protocolheader.CopyTo(header, 25);
            _msgtype.CopyTo(header, 33);
            _reservedfield2.CopyTo(header, 35);

            return header;
        }
    }
}
