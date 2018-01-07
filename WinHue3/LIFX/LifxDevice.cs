using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX.Responses.States.Device;

namespace WinHue3.LIFX
{
    public abstract class LifxDevice 
    {
        protected IPAddress _ip;
        protected byte[] _mac;

        protected LifxDevice(IPAddress ip, byte[] mac)
        {
            _ip = ip;
            _mac = mac;
        }

        /// <summary>
        /// Set the power state of the device.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="target">(Optional) Mac address of the targetted device</param>
        /// <returns>The power level from 0 to 65535</returns>
        public async Task<CommMessage<LifxResponse>> SetPowerAsync(ushort level, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new PowerPayload(level, transitiontime);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the location of the device
        /// </summary>
        /// <param name="location">GUID location of the device</param>
        /// <param name="label">Label of the location</param>
        /// <param name="updatedAt">Time of the Update</param>
        /// <returns>The response of the device.</returns>
        public async Task<CommMessage<LifxResponse>> SetLocationAsync(Guid location,string label, DateTime updatedAt)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLocation);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLocation(location,label,updatedAt);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the power state of the device.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="target">(Optional) Mac address of the targetted device</param>
        /// <returns>The power level from 0 to 65535</returns>
        public CommMessage<LifxResponse> SetPower(ushort level, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new PowerPayload(level, transitiontime);
            CommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the label of the device.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public async Task<CommMessage<LifxResponse>> SetLabel(string label)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLabel(label);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

    }
}
