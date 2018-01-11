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
            Device_GetVersion = 32,
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
            _header = new byte[2] { 0x00, 0x14 };
            _uniqueid = new byte[4] { 0x30, 0x33, 0x48, 0x57 }; // WH30 (reversed)
            _target = new byte[8]; // MAC ADDRESS OF THE DEVICE OR ALL ZERO FOR ALL 

            _reservedfield1 = new byte[6]; // MUST BE ZERO
            _ack = new byte[1] { 0x03 };
            _sequence = new byte[1] { 0x00 };
            _protocolheader = new byte[8]; // MUST BE ZERO
            _msgtype = new byte[2];
            _reservedfield2 = new byte[2]; // MUST BE ZERO

        }

        public Header(byte[] bytes)
        {
            _size = new byte[2];
            _header = new byte[2] { 0x00, 0x14 };
            _uniqueid = new byte[4] { 0x30, 0x33, 0x48, 0x57 }; // WH30 (reversed)
            _target = new byte[8]; // MAC ADDRESS OF THE DEVICE OR ALL ZERO FOR ALL 

            _reservedfield1 = new byte[6]; // MUST BE ZERO
            _ack = new byte[1] { 0x03 };
            _sequence = new byte[1] { 0x00 };
            _protocolheader = new byte[8]; // MUST BE ZERO
            _msgtype = new byte[2];
            _reservedfield2 = new byte[2]; // MUST BE ZERO

            Array.Copy(bytes,0,_size,0,_size.Length);
            Array.Copy(bytes,2,_header,0,_header.Length);
            Array.Copy(bytes,4,_uniqueid,0,_uniqueid.Length);
            Array.Copy(bytes,8,_target,0,_target.Length);
            Array.Copy(bytes,16,_reservedfield1,0,_reservedfield1.Length);
            Array.Copy(bytes,22,_ack,0,_ack.Length);
            Array.Copy(bytes,23,_sequence,0,_sequence.Length);
            Array.Copy(bytes,24,_protocolheader,0,_protocolheader.Length);
            Array.Copy(bytes,32,_msgtype,0,_msgtype.Length);
            Array.Copy(bytes,34,_reservedfield2,0,_reservedfield2.Length);

        }

        public ushort Length => 
            Convert.ToUInt16(_size.Length + _header.Length + _uniqueid.Length + _target.Length + 
                _reservedfield1.Length + _ack.Length + _sequence.Length + _protocolheader.Length + _msgtype.Length + _reservedfield2.Length);

        public byte[] Target => _target.Reverse().ToArray();
        public MessageType MsgType => (MessageType)BitConverter.ToUInt16(_msgtype, 0);
        public bool Ack => Convert.ToByte(_ack[0] & 0x02) == 0x02;
        public bool Response => Convert.ToByte(_ack[0] & 0x01) == 0x01;
        public bool Tagged => Convert.ToByte(_header[1] & 0x34) == 0x34;
        public string UniqueID => Encoding.UTF8.GetString(_uniqueid.Reverse().ToArray());


        public void SetSize(ushort size)
        {
            BitConverter.GetBytes(size).ToArray().CopyTo(_size, 0);
        }

        public void SetTagged(bool tagged)
        {
            if (tagged)
            {
                _header[1] = 0x34;
            }
            else
            {
                _header[1] = 0x14;
            }
        }

        public void SetTargetMAC(byte[] mac)
        {
            mac.Reverse().ToArray().CopyTo(_target,0);            
        }

        public void SetMessageType(MessageType msgtype)
        {
            BitConverter.GetBytes((ushort)msgtype).ToArray().CopyTo(_msgtype, 0);
        }

        public void SetAck(bool ack)
        {
            if (ack)
            {
                _ack[0] = Convert.ToByte(_ack[0] | 0x02);
            }
            else
            {
                _ack[0] = Convert.ToByte(_ack[0] & 0x01);
            }
        }

        public void SetResponse(bool resp)
        {
            if (resp)
            {
                _ack[0] = Convert.ToByte(_ack[0] | 0x01);
            }
            else
            {
                _ack[0] = Convert.ToByte(_ack[0] & 0x02);
            }
        }


        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];

            //**** FRAME HEADER
            // SIZE is 0 - 1 and is the size of the whole message (including size)

            _header.CopyTo(bytes, 2); // 2 - 3
            _uniqueid.CopyTo(bytes, 4); // 4 - 7

            //**** FRAME ADDRESS
            _target.CopyTo(bytes, 8); // 8 - 15
            _reservedfield1.CopyTo(bytes, 16); // 16 - 21
            _ack.CopyTo(bytes, 22);
            _sequence.CopyTo(bytes, 23);
            _protocolheader.CopyTo(bytes, 24);
            _msgtype.CopyTo(bytes, 32);
            _reservedfield2.CopyTo(bytes, 33);

            return bytes;
        }

        public static implicit operator byte[](Header header)
        {
            return header.GetBytes();
        }
    }
}
