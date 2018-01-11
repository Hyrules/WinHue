using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;

namespace WinHue3.LIFX.Responses.States.Device
{
    public class StateInfo : Payload
    {
        private readonly byte[] _time;
        private readonly byte[] _uptime;
        private readonly byte[] _downtime;

        public DateTime Time => new DateTime(1970,1,1,0,0,0).AddSeconds(BitConverter.ToDouble(_time,0));
        public TimeSpan UpTime => new TimeSpan(BitConverter.ToInt64(_uptime,0));
        public TimeSpan DownTime => new TimeSpan(BitConverter.ToInt64(_downtime, 0));

        public StateInfo()
        {
            _time = new byte[8];
            _uptime = new byte[8];
            _downtime = new byte[8];
        }

        public StateInfo(byte[] bytes) : this()
        {
            Array.Copy(bytes,0,_time,0,_time.Length);
            Array.Copy(bytes,8,_uptime,0,_uptime.Length);
            Array.Copy(bytes,16,_downtime,0,_downtime.Length);

        }

        public override int Length => _time.Length + _uptime.Length + _downtime.Length;

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            _time.CopyTo(bytes,0);
            _uptime.CopyTo(bytes,8);
            _downtime.CopyTo(bytes,16);
            return bytes;

        }
    }
}
