using System;
using WinHue3.LIFX.Framework.Colors;

namespace WinHue3.LIFX.Framework.Payloads
{
    public class WaveformOptionalPayload : WaveformPayload
    {
        private byte[] _sethue;
        private byte[] _setsat;
        private byte[] _setbri;
        private byte[] _setkelvin;


        public WaveformOptionalPayload() : base()
        {
            _sethue = new byte[1];
            _setsat = new byte[1];
            _setbri = new byte[1];
            _setkelvin = new byte[1];
        }

        public WaveformOptionalPayload(bool transient, Hsbk color, uint period, float cycles, short skewratio, byte waveform, bool sethue,bool setbri, bool setsat, bool setkelvin) : base(transient,color, period, cycles,skewratio, waveform)
        {
            _setbri = BitConverter.GetBytes(setbri);
            _setsat = BitConverter.GetBytes(setsat);
            _sethue = BitConverter.GetBytes(sethue);
            _setkelvin = BitConverter.GetBytes(setkelvin);
        }

        public override int Length =>
            base.Length + _setbri.Length + _setkelvin.Length + _sethue.Length + _setsat.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            base.GetBytes().CopyTo(bytes,0);
            _sethue.CopyTo(bytes,21);
            _setsat.CopyTo(bytes,22);
            _setbri.CopyTo(bytes,23);
            _setkelvin.CopyTo(bytes,24);
            return bytes;
        }
    }
}
