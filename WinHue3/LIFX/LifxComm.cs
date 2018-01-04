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
        private byte[] header;
        private byte[] destination;
        private byte[] source;
        private byte[] reservedfield1;
        private byte ack;
        private byte sequence;
        private byte[] protocolheader;
        private byte[] message;
        private byte[] reservedfield2;

        //*** PAYLOAD ***

        

        public LifxPacket()
        {
            header = new byte[2];
            destination = new byte[4];
            source = new byte[4];
            
            reservedfield1 = new byte[6]; // MUST BE ZERO
            ack = 0xFF;
            sequence = 0xFF;
            protocolheader = new byte[8]; // MUST BE ZERO
            reservedfield2 = new byte[2]; // MUST BE ZERO

        }

        public byte[] MakePacket()
        {
            byte[] packet = new byte[49];
            

            return packet;

        }

    }
}
