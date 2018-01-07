using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Payloads
{
    public class PowerPayload : Payload
    {
        private byte[] _level;
        private byte[] _duration;

        public PowerPayload()
        {
            _level = new byte[2];
            _duration = new byte[4];
        }

        public PowerPayload(ushort level,uint duration)
        {
            _level = new byte[2];
            _duration = new byte[4];
            BitConverter.GetBytes(level).ToArray().CopyTo(_level, 0);
            BitConverter.GetBytes(duration).ToArray().CopyTo(_duration, 0);
        }

        public PowerPayload(byte[] bytes)
        {
            _level = new byte[2];
            _duration = new byte[4];
            Array.Copy(bytes,0,_level,0,_level.Length);
            Array.Copy(bytes,2,_duration,0,_duration.Length);
        }

        public override int Length => _level.Length + _duration.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[_level.Length + _duration.Length];
            Array.Copy(_level, 0, bytes, 0, _level.Length);
            Array.Copy(_duration, 0, bytes, 2, _duration.Length);
            return bytes;
        }
    }
}
