using System;

namespace WinHue3.LIFX.Responses.States.Device
{
    public class StateService
    {
        private readonly byte[] _service;
        private readonly byte[] _port;
        private readonly Header _hdr;

        public StateService(Header hdr,byte[] bytes)
        {
            _hdr = hdr;
            _service = new byte[1];
            _port = new byte[4];
            Array.Copy(bytes,0,_service,0,_service.Length);
            Array.Copy(bytes,1,_port,0,_port.Length);
        }

        public ushort Port => BitConverter.ToUInt16(_port,0);
        public bool Udp => _service[0] == 0x01;
        public Header Header => _hdr;
    }
}