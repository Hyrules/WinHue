using System;
using WinHue3.LIFX.Framework.Payloads;

namespace WinHue3.LIFX.Framework.Responses.States.Device
{
    public class StateHostInfo : Payload
    {
        private readonly byte[] _signal;
        private readonly byte[] _tx;
        private readonly byte[] _rx;
        private readonly byte[] _reserved;

        public StateHostInfo()
        {
            _signal = new byte[4];
            _tx = new byte[4];
            _rx = new byte[4];
            _reserved = new byte[2];
            
        }

        public StateHostInfo(byte[] bytes) : this()
        {
            Array.Copy(bytes, 0, _signal,0, _signal.Length);
            Array.Copy(bytes, 4, _tx, 0, _tx.Length);
            Array.Copy(bytes, 8, _rx, 0, _rx.Length);
            Array.Copy(bytes, 12, _reserved, 0, _reserved.Length);
        }

        public override int Length => _signal.Length + _tx.Length + _rx.Length + _reserved.Length;

        public byte[] Signal => _signal;

        public byte[] Tx => _tx;

        public byte[] Rx => _rx;

        public byte[] Reserved => _reserved;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _signal.CopyTo(bytes, 0);
            _tx.CopyTo(bytes,4);
            _rx.CopyTo(bytes,8);
            _reserved.CopyTo(bytes,12);
            return bytes;
        }
    }
}

