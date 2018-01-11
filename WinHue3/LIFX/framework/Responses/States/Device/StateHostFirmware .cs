using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Device
{
    public class StateHostFirmware : Payload
    {
        private readonly byte[] _build;
        private readonly byte[] _reserved;
        private readonly byte[] _version;


        public StateHostFirmware()
        {
            _build = new byte[8];
            _reserved = new byte[8];
            _version = new byte[2];
        }

        public StateHostFirmware(byte[] bytes) : this()
        {

            Array.Copy(bytes,0,_build,0,_build.Length);
            Array.Copy(bytes,8,_reserved,0,_reserved.Length);
            Array.Copy(bytes,16,_version,0,_version.Length);
        }

        public override int Length => _build.Length + _reserved.Length + _version.Length;

        public byte[] Build => _build;

        public byte[] Reserved => _reserved;

        public byte[] Version => _version;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _build.CopyTo(bytes,0);
            _reserved.CopyTo(bytes,8);
            _version.CopyTo(bytes, 16);
            return bytes;
        }
    }
}
