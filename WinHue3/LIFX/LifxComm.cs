using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace WinHue3.LIFX
{
    public static class LifxComm
    {
        public enum LightMessageType {
            Get =0x65,
            SetColor =0x66,
            SetWaveForm =0x67,
            SetWaveFormOptional =0x77,
            State = 0x6B,
            GetPower = 0x74,
            SetPower = 0x75,
            GetInfrared = 0x78,
            StateInfrared = 0x79,
            SetInfrared = 0x7A
        };

        public enum MultiZoneMessageType
        {
            SetColorZones = 0x1F5,
            GetcolorZones = 0x1F6,
            StateZones = 0x1F7,
            StateMultiZone = 0x1FA
        };

        private static UdpClient _udpclient;

        static LifxComm()
        {
            _udpclient = new UdpClient();
        }

        private async static Task SendPacketAsync(LifxPacket packet)
        {
            byte[] currentpacket = packet;
            await _udpclient.SendAsync(currentpacket, currentpacket.Length);
        }
    }

    public class LifxPacket
    {

        //*** FRAME HEADER ***
        private byte[] _header;
        private byte[] _uniqueid;

        //*** FRAME ADDRESS ***
        private byte[] _target;      
        private byte[] _reservedfield1;
        private byte[]  _ack;
        private byte[]  _sequence;
        private byte[] _protocolheader;
        private byte[] _msgtype;
        private byte[] _reservedfield2;        

        //*** PAYLOAD ***



        public LifxPacket()
        {

            _header   = new byte[4] { 0x00,0x00,0x34,0x00 };
            _uniqueid = new byte[4] { 0x56,0x48,0x03,0x00 }; // WH30
            _target   = new byte[8];
            
            _reservedfield1 = new byte[6]; // MUST BE ZERO
            _ack = new byte[1] { 0x11 };
            _sequence = new byte[1] { 0x00 };
            _protocolheader = new byte[8]; // MUST BE ZERO
            _msgtype = new byte[2];
            _reservedfield2 = new byte[2]; // MUST BE ZERO

        }

        public byte[] MakePacket()
        {
            byte[] packet = new byte[16];
            
            //**** FRAME HEADER
            Array.Copy(_header, packet, _header.Length); // 0 - 4
            Array.Copy(_uniqueid,0, packet,5, _uniqueid.Length); // 5 - 9

            //**** FRAME ADDRESS
            Array.Copy(_target,0, packet,10, _target.Length); // 10 - 17
            Array.Copy(_reservedfield1,0, packet,18, _reservedfield1.Length); // 18 - 23
            Array.Copy(_ack,0, packet, 24, _ack.Length); // 24 
            Array.Copy(_sequence,0, packet,25, _sequence.Length); // 25
            Array.Copy(_protocolheader,0, packet, 26, _protocolheader.Length); // 26 - 33
            Array.Copy(_msgtype,0, packet,34, _msgtype.Length); // 34 - 35
            Array.Copy(_reservedfield2,0, packet,36, _reservedfield2.Length); // 36 - 37

            //**** PAYLOAD 


            // SET FINAL SIZE
            return packet;
        }

        public static implicit operator byte[](LifxPacket packet)
        {
            return packet.MakePacket();
        }

        public void SetTargetIP(IPAddress ip)
        {
            _target = ip.GetAddressBytes();
        }

        public void SetTargetIP(String ip)
        {
            _target = IPAddress.Parse(ip).GetAddressBytes();
        }
    }
}
