using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Device
{
    public class StateGroup : Payload
    {
        private readonly byte[] _group;
        private readonly byte[] _label;
        private readonly byte[] _updatedat;

        public Guid Location => new Guid(_group.Reverse().ToArray());
        public string Label => Encoding.UTF8.GetString(_label).TrimEnd('\0');
        public DateTime UpdatedAt => new DateTime(1970,1,1,0,0,0).AddSeconds(BitConverter.ToDouble(_updatedat,0));

        public StateGroup()
        {
            _group = new byte[16];
            _label = new byte[32];
            _updatedat = new byte[64];
        }

        public StateGroup(byte[] bytes) : this()
        {
            Array.Copy(bytes,0,_group,0,_group.Length);
            Array.Copy(bytes,16,_label,0,_label.Length);
            Array.Copy(bytes,48,_updatedat,0,_updatedat.Length);
        }

        public override int Length => _group.Length + _label.Length + _updatedat.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _group.CopyTo(bytes,0);
            _label.CopyTo(bytes,16);
            _updatedat.CopyTo(bytes,48);
            return bytes;
        }
    }
}
