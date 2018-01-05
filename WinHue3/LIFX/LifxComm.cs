using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Controls;

namespace WinHue3.LIFX
{
    public static class LifxComm
    {
        public enum MessagesType {
            Get =0x65,
            SetColor =0x66,
            SetWaveForm =0x67,
            SetWaveFormOptional =0x77,
            State = 0x6B,
            GetPower = 0x74,
            SetPower = 0x75,
            GetInfrared = 0x78,
            StateInfrared = 0x79,
            SetInfrared = 0x7A,
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

        public static async Task SendPacketAsync(LifxPacket packet)
        {
            byte[] currentpacket = packet;
            await _udpclient.SendAsync(currentpacket, currentpacket.Length);
        }

        public static void SendPacket(LifxPacket packet)
        {
            byte[] currentpacket = packet;
            _udpclient.Send(currentpacket, currentpacket.Length);
        }
    }

    public class LifxPacket
    {
        // LIFX USES LITTLE ENDIAN THIS MEANS THAT THE MOST SIGNIFICAT BIT IS REVERSE (least significant byte is stored first)
        // EG : color 

        //*** FRAME HEADER ***
        private byte[] _header;
        private byte[] _uniqueid;

        //*** FRAME ADDRESS ***
        private byte[] _target;      
        private byte[] _reservedfield1;
        private byte[]  _ack;
        private byte[]  _sequence;
        private byte[] _protocolheader;
        private byte[] _reservedfield2;
        private byte[] _msgtype;
        private byte[] _reservedfield3;

        //*** PAYLOAD ***
        private byte[] _color;
        private byte[] _sat;
        private byte[] _bri;
        private byte[] _kelvin;
        private byte[] _transition;
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
            _reservedfield3 = new byte[2]; // MUST BE ZERO

            _color = new byte[2];
            _sat = new byte[2];
            _bri = new byte[2];
            _kelvin = new byte[2];
            _transition = new byte[4];
        }


        private byte[] MakePacket(byte[] payload)
        {
            byte[] packet = new byte[42 + payload.Length];
            
            //**** FRAME HEADER
            _header.CopyTo(packet,0); // 0 - 3
            _uniqueid.CopyTo(packet,4); // 4 - 8
            
            //**** FRAME ADDRESS
            _target.CopyTo(packet,9); // 9 - 16
            _reservedfield1.CopyTo(packet,17); // 17 - 22 
            _ack.CopyTo(packet, 23);
            _sequence.CopyTo(packet,24);
            _protocolheader.CopyTo(packet, 25);
            _msgtype.CopyTo(packet,33);
            _reservedfield2.CopyTo(packet,35);

            //**** PAYLOAD 

            payload.CopyTo(packet,43);

            // SET FINAL SIZE

            
            

            return packet;
        }

        public void SetColor(uint color, uint bri, uint sat, uint kelvin, uint transitiontime)
        {
            byte[] payload = new byte[8];
            SetBytes(ref _color, color);
            _color.CopyTo(payload, 0); // 0 -1
            SetBytes(ref _sat, sat);
            _sat.CopyTo(payload, 2); // 2 - 3
            SetBytes(ref _bri, bri);
            _bri.CopyTo(payload, 4); // 4 - 5
            SetBytes(ref _kelvin, kelvin);
            _kelvin.CopyTo(payload, 6); // 6 - 7
            SetBytes(ref _transition, transitiontime);
            _transition.CopyTo(payload, 8);
        }

        public void SetBytes(ref byte[] array, uint value)
        {
            BitConverter.GetBytes(value).Reverse().ToArray().CopyTo(array,0);
        }

        public void SetMessageType(LifxComm.MessagesType msgtype)
        {
            _msgtype[0] = (byte)msgtype;
        }

        public static implicit operator byte[](LifxPacket packet)
        {
            return new byte[2];
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
