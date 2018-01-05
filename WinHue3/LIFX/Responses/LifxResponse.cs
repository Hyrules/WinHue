using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Responses
{
    public class LifxResponse
    {
        public Acknowledgement ack { get; set; }
        public byte[] data { get; set; }
    }
}
