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

        #region GET_WIFI_INFO
        /// <summary>
        /// Get Wifi information async.
        /// </summary>
        /// <returns>Requested information.</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetWifiInfoAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetWifiInfo);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get wifi information.
        /// </summary>
        /// <returns>Requested information.</returns>
        public LifxCommMessage<LifxResponse> GetWifiInfo()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetWifiInfo);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region GET_HOST_FIRMWARE
        /// <summary>
        /// Get Host firmware async.
        /// </summary>
        /// <returns>Response to the command.</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetHostFirmwareAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetHostFirmware);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response; 
        }

        /// <summary>
        /// Get Host firmware.
        /// </summary>
        /// <returns>Response to the command.</returns>
        public LifxCommMessage<LifxResponse> GetHostFirmware()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetHostFirmware);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region GET_INFO

        /// <summary>
        /// Get the info of the device async.
        /// </summary>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetInfoAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetInfo);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the Group of the device.
        /// </summary>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> GetInfo()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetInfo);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region GET_VERSION
        /// <summary>
        /// Get version async.
        /// </summary>
        /// <returns>Requested information.</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetVersionAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetVersion);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get version.
        /// </summary>
        /// <returns>Requested information.</returns>
        public LifxCommMessage<LifxResponse> GetVersion()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetVersion);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }
        #endregion

        #region GET_SET_POWER
        /// <summary>
        /// Set the power state of the device.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <returns>The power level from 0 to 65535</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetPowerAsync(ushort level, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new PowerPayload(level, transitiontime);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the power state of the device.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <returns>The power level from 0 to 65535</returns>
        public LifxCommMessage<LifxResponse> SetPower(ushort level, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new PowerPayload(level, transitiontime);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the power state of the device async.
        /// </summary>
        /// <returns>The power level from 0 to 65535</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetPowerAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the power state of the device.
        /// </summary>
        /// <returns>The power level from 0 to 65535</returns>
        public LifxCommMessage<LifxResponse> GetPower()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetPower);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region GET_SET_LABEL
        /// <summary>
        /// Set the label of the device.
        /// </summary>
        /// <param name="label">Label of the device.</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetLabelAsync(string label)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLabel(label);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the label of the device.
        /// </summary>
        /// <param name="label">Label of the device</param>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> SetLabel(string label)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLabel(label);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the label of the device.
        /// </summary>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetLabelAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the label of the device.
        /// </summary>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> GetLabel()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region GET_SET_LOCATION
        /// <summary>
        /// Set the location of the device
        /// </summary>
        /// <param name="location">GUID location of the device</param>
        /// <param name="label">Label of the location</param>
        /// <param name="updatedAt">Time of the Update</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetLocationAsync(Guid location,string label, DateTime updatedAt)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLocation);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLocation(location,label,updatedAt);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the location of the device
        /// </summary>
        /// <param name="location">GUID location of the device</param>
        /// <param name="label">Label of the location</param>
        /// <param name="updatedAt">Time of the Update</param>
        /// <returns>The response of the device.</returns>
        public LifxCommMessage<LifxResponse> SetLocation(Guid location, string label, DateTime updatedAt)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLocation);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateLocation(location, label, updatedAt);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the location of the device async.
        /// </summary>
        /// <returns>Requested information.</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetLocationAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLocation);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the location of the device.
        /// </summary>
        /// <returns>Requested information.</returns>
        public LifxCommMessage<LifxResponse> GetLocation()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLocation);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }
        #endregion

        #region GET_SET_GROUP

        /// <summary>
        /// Set the Group of the device.
        /// </summary>
        /// <param name="label">Label of the device.</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetGroupAsync(Guid guid, string label, DateTime timestamp)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateGroup(guid,label,timestamp);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the Group of the device.
        /// </summary>
        /// <param name="label">Label of the device</param>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> SetGroup(Guid guid, string label, DateTime timestamp)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_SetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new StateGroup(guid,label,timestamp);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the Group of the device.
        /// </summary>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> GetGroupAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the Group of the device.
        /// </summary>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> GetGroup()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetLabel);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion
    }
}
