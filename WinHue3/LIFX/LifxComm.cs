using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace WinHue3.LIFX
{
    public static class LifxComm
    {

    }

    public class LifxPacket
    {

        //*** HEADER ***
        private byte[] _header;
        private byte[] _destination;
        private byte[] _source;
        private byte[] _reservedfield1;
        private byte   _ack;
        private byte   _sequence;
        private byte[] _protocolheader;
        private byte[] _message;
        private byte[] _reservedfield2;

        //*** PAYLOAD ***

        

        public LifxPacket()
        {
            _header = new byte[2];
            _destination = new byte[4];
            _source = new byte[4];
            
            _reservedfield1 = new byte[6]; // MUST BE ZERO
            _ack = 0xFF;
            _sequence = 0xFF;
            _protocolheader = new byte[8]; // MUST BE ZERO
            _reservedfield2 = new byte[2]; // MUST BE ZERO

        }

        public byte[] MakePacket()
        {
            byte[] packet = new byte[49];
            

            return packet;

        }

    }
}
