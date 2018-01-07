using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX;

namespace WinHue3.LIFX
{
    public class LifxLight : LifxDevice
    {
        private State _state;

        private LifxLight(IPAddress ip, byte[] mac) : base(ip, mac)
        {

        }

        public Hsbk Color => _state.HSBK;
        public ushort Power => _state.Power;
        public string Label => _state.Label;

        public static async Task<LifxLight> CreateLight(IPAddress ip, byte[] mac)
        {
            LifxLight light = new LifxLight(ip,mac);
            CommMessage<State> msg = await light.GetStateAsync();
            if (!msg.Error)
            {
                light._state = msg.Data;
            }

            return light;
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
        public async Task<CommMessage<LifxResponse>> SetColorAsync(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(color, bri, sat, kelvin, transitiontime);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="hsbk">Hue, saturation , brightness, kelvin color object</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip"></param>
        /// <returns>(Optional) IP address of the targetted device</returns>
        public async Task<CommMessage<LifxResponse>> SetColorAsync(Hsbk hsbk, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(hsbk, transitiontime);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
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
        public CommMessage<LifxResponse> SetColor(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(color, bri, sat, kelvin, transitiontime);
            CommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="hsbk">Hue, saturation , brightness, kelvin color object</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip"></param>
        /// <returns>(Optional) IP address of the targetted device</returns>
        public CommMessage<LifxResponse> SetColor(Hsbk hsbk, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(hsbk, transitiontime);
            CommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Get the state of the specified light
        /// </summary>
        /// <param name="ip">IP address of the light</param>
        /// <returns>State of the light</returns>
        public async Task<CommMessage<State>> GetStateAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_Get);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            if (response.Error)
            {
                return new CommMessage<State>(response.Ex,null,true);
            }
            else
            {
                CommMessage<State> finalresponse = new CommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
           
        }

    }
}
