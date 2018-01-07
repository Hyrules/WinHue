using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Device
{
    public class StateLocation : Payload
    {
        private readonly byte[] _location;
        private readonly byte[] _label;
        private readonly byte[] _updatedat;

        public Guid Location => new Guid(_location.Reverse().ToArray());
        public string Label => Encoding.UTF8.GetString(_label).TrimEnd('\0');
        public DateTime UpdatedAt => new DateTime(1970,1,1,0,0,0).AddSeconds(BitConverter.ToDouble(_updatedat,0));

        public StateLocation()
        {
            _location = new byte[16];
            _label = new byte[32];
            _updatedat = new byte[64];
        }

        public StateLocation(byte[] bytes) : this()
        {
            Array.Copy(bytes,0,_location,0,_location.Length);
            Array.Copy(bytes,16,_label,0,_label.Length);
            Array.Copy(bytes,48,_updatedat,0,_updatedat.Length);
        }

        public StateLocation(Guid location, string label, DateTime updatedAt)
        {
            _location = location.ToByteArray().Reverse().ToArray();
            _label = Encoding.UTF8.GetBytes(label).Reverse().ToArray();
            _updatedat = BitConverter.GetBytes((UInt64) updatedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

        }

        public override int Length => _location.Length + _label.Length + _updatedat.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _location.CopyTo(bytes,0);
            _label.CopyTo(bytes,16);
            _updatedat.CopyTo(bytes,48);
            return bytes;
        }
    }
}
