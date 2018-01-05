using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Controls;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;

namespace WinHue3.LIFX
{
    public static class LifxComm
    {
        private static UdpClient _udpClient;

        static LifxComm()
        {
            _udpClient = new UdpClient();            
        }

        private static async Task<LifxResponse> SendPacketAsync(IPAddress ip , LifxPacket packet)
        {
            byte[] currentPacket = packet;
            IPEndPoint clientIp = new IPEndPoint(ip ?? IPAddress.Broadcast,56700);
            if(clientIp.Address != IPAddress.Broadcast)
            {
                _udpClient.Connect(clientIp);             
            }

            await _udpClient.SendAsync(currentPacket, currentPacket.Length, clientIp);
            
            UdpReceiveResult udpAck = await _udpClient.ReceiveAsync();
            UdpReceiveResult udpResponse = await _udpClient.ReceiveAsync();

            LifxResponse response = new LifxResponse()
            {
                ack = new Acknowledgement(udpAck.Buffer),
                data = udpResponse.Buffer,
            };
                     
            _udpClient.Close();
            return response;
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="color">Color Hue from 0 to 65535</param>
        /// <param name="bri">Brightness from 0 to 65535</param>
        /// <param name="sat">Saturation from 0 to 65535</param>
        /// <param name="kelvin">Saturation in kelvin from 0 (2500 Deg) to 65535 (9000 Deg)</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip">(Optional) Ip address of the targetted device</param>
        /// <returns>State of the light</returns>
        public static async Task<State> SetColorAsync(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime, IPAddress ip = null)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTargetIP(ip);
            packet.Payload = new ColorPayload(color, bri, sat, kelvin, transitiontime);
            LifxResponse response = await SendPacketAsync(ip,packet);
            return new State(response.data);
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="hsbk">Hue, saturation , brightness, kelvin color object</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip"></param>
        /// <returns>(Optional) IP address of the targetted device</returns>
        public static async Task<State> SetColorAsync(Hsbk hsbk, uint transitiontime, IPAddress ip = null)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTargetIP(ip);
            packet.Payload = new ColorPayload(hsbk, transitiontime);
            LifxResponse response = await SendPacketAsync(ip, packet);
            return new State(response.data);
        }

        /// <summary>
        /// Set the power state of one or all light.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="target">(Optional) Mac address of the targetted device</param>
        /// <returns>The power level from 0 to 65535</returns>
        public static async Task<ushort> SetPower(ushort level, uint transitiontime, IPAddress ip = null)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTargetIP(ip);
            packet.Payload = new PowerPayload(level, transitiontime);
            LifxResponse response = await SendPacketAsync(ip, packet);
            return BitConverter.ToUInt16(response.data,0);
        }

        /// <summary>
        /// Get the state of the specified light
        /// </summary>
        /// <param name="ip">IP address of the light</param>
        /// <returns>State of the light</returns>
        public static async Task<State> GetState(IPAddress ip)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_Get);
            packet.Header.SetTargetIP(ip);
            packet.Header.SetSize(packet.Header.Length);
            LifxResponse response = await SendPacketAsync(ip, packet);
            return new State(response.data);
        }
    }

}
