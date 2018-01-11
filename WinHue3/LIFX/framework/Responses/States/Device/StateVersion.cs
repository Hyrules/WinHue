using System;
using WinHue3.LIFX.Framework.Payloads;

namespace WinHue3.LIFX.Framework.Responses.States.Device
{
    public class StateVersion : Payload
    {
        private readonly byte[] _vendor;
        private readonly byte[] _product;
        private readonly byte[] _version;

        public StateVersion()
        {
            _vendor = new byte[4];
            _product = new byte[4];
            _version = new byte[4];
        }

        public StateVersion(byte[] bytes) : this()
        {

            Array.Copy(bytes,0,_vendor,0,_vendor.Length);
            Array.Copy(bytes,4,_product,0,_product.Length);
            Array.Copy(bytes,8,_version,0,_version.Length);
        }

        public override int Length => _vendor.Length + _product.Length + _version.Length;

        public byte[] Vendor => _vendor;

        public byte[] Product => _product;

        public byte[] Version1 => _version;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _vendor.CopyTo(bytes,0);
            _product.CopyTo(bytes,4);
            _version.CopyTo(bytes,8);
            return bytes;
        }
    }
}
