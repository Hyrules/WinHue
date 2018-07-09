using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Philips_Hue.BridgeObject.Entertainment_API
{
    public class StreamMessage
    {
        public enum ColorSpace { RGB = 0x00, XYBrightness = 0x01}

        byte[] _protocolname;
        byte[] _version;
        byte[] _sequenceId;
        byte[] _reserved;
        byte[] _colorspace;
        byte[] _reserved2;
        byte[] _data;
       

        public StreamMessage()
        {
            _protocolname = new byte[9];
            _protocolname = Encoding.UTF8.GetBytes("HueStream");
            _version = new byte[2] { 0x01, 0x00 };
            _sequenceId = new byte[1];
            _reserved = new byte[2] { 0x00, 0x00 };
            _colorspace = new byte[1] { (byte)ColorSpace.RGB };
            _reserved2 = new byte[1] { 0x00 };
            _data = new byte[9];
        }

        public int Length => _protocolname.Length + _version.Length + _sequenceId.Length + _reserved.Length + _colorspace.Length + _reserved2.Length + _data.Length;

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];

            return bytes;
        }

        public static implicit operator byte[](StreamMessage msg)
        {
            return msg.GetBytes();
        }
    }
}
