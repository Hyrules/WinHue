using System;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Light
{
    public class StateBrightness : Payload
    {
        private readonly byte[] _brightness;

        public StateBrightness()
        {
            _brightness = new byte[2];
        }

        public StateBrightness(byte[] bytes)
        {
            _brightness = bytes;
        }

        public StateBrightness(ushort bri)
        {
            _brightness = BitConverter.GetBytes(bri);
        }

        public override int Length => _brightness.Length;
        public ushort Brightness => BitConverter.ToUInt16(_brightness,0);

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _brightness.CopyTo(bytes,0);
            return bytes;
        }
    }
}
