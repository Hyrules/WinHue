using System;
using WinHue3.LIFX.Framework.Payloads;

namespace WinHue3.LIFX.Framework.Responses.States.Device
{
    public class StatePower : Payload
    {
        private readonly byte[] _level;

        public StatePower()
        {
            _level = new byte[2];
        }

        public StatePower(byte[] bytes) : this()
        {
            Array.Copy(bytes,0,_level,0,_level.Length);
        }

        public override int Length => _level.Length;

        public ushort Level => BitConverter.ToUInt16(_level,0);

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _level.CopyTo(bytes,0);
            return bytes;

        }
    }
}