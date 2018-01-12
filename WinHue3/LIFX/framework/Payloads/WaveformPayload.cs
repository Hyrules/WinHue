using System;
using WinHue3.LIFX.Framework.Colors;

namespace WinHue3.LIFX.Framework.Payloads
{
    public class WaveformPayload : Payload
    {
        private byte[] _reserved;
        private byte[] _transient;
        private byte[] _hsbk;
        private byte[] _period;
        private byte[] _cycles;
        private byte[] _skewratio;
        private byte[] _waveform;


        public WaveformPayload()
        {
            _reserved = new byte[1];
            _transient = new byte[1];
            _hsbk = new Hsbk();
            _period = new byte[4];
            _cycles = new byte[4];
            _skewratio = new byte[2];
            _waveform = new byte[1];
        }

        public WaveformPayload(bool transient, Hsbk color, uint period, float cycles, short skewratio, byte waveform) : this()
        {
            _transient = BitConverter.GetBytes(transient);
            _hsbk = color;
            _period = BitConverter.GetBytes(period);
            _cycles = BitConverter.GetBytes(cycles);
            _skewratio = BitConverter.GetBytes(skewratio);
            _waveform = BitConverter.GetBytes(waveform);
        }

        public override int Length => _reserved.Length + _transient.Length + _hsbk.Length + _period.Length + _cycles.Length + _skewratio.Length + _waveform.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _reserved.CopyTo(bytes,0);
            _transient.CopyTo(bytes,1);
            _hsbk.CopyTo(bytes,2);
            _period.CopyTo(bytes,10);
            _cycles.CopyTo(bytes,14);
            _skewratio.CopyTo(bytes,18);
            _waveform.CopyTo(bytes,20);
            return bytes;
        }
    }
}
