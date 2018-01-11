using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX;
using WinHue3.LIFX.Responses.States.Light;

namespace WinHue3.LIFX
{
    public class LifxLight : LifxDevice
    {
        private State _state;

        public LifxLight(IPAddress ip, byte[] mac) : base(ip, mac)
        {

        }

        public Hsbk Color => _state.HSBK;
        public ushort Power => _state.Power;
        public string Label => _state.Label;

        public static async Task<LifxLight> CreateLight(IPAddress ip, byte[] mac)
        {
            LifxLight light = new LifxLight(ip,mac);
            LifxCommMessage<State> msg = await light.GetStateAsync();
            if (!msg.Error)
            {
                light._state = msg.Data;
            }

            return light;
        }

        #region SET_COLOR
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
        public async Task<LifxCommMessage<LifxResponse>> SetColorAsync(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(color, bri, sat, kelvin, transitiontime);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="hsbk">Hue, saturation , brightness, kelvin color object</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip"></param>
        /// <returns>(Optional) IP address of the targetted device</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetColorAsync(Hsbk hsbk, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(hsbk, transitiontime);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
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
        public LifxCommMessage<LifxResponse> SetColor(ushort color, ushort bri, ushort sat, ushort kelvin, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(color, bri, sat, kelvin, transitiontime);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set one or all lights to a specific color
        /// </summary>
        /// <param name="hsbk">Hue, saturation , brightness, kelvin color object</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="ip"></param>
        /// <returns>(Optional) IP address of the targetted device</returns>
        public LifxCommMessage<LifxResponse> SetColor(Hsbk hsbk, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetColor);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Payload = new ColorPayload(hsbk, transitiontime);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion

        #region SET_WAVEFORM

        /// <summary>
        /// Set the waveform of the light async.
        /// </summary>
        /// <param name="transient">Color does not persist</param>
        /// <param name="color">Light end color</param>
        /// <param name="period">Duration of a cycle in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewratio">Waveform Skew, [-32768, 32767] scaled to [0, 1]</param>
        /// <param name="waveform">Waveform to use for transition.</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<State>> SetWaveformAsync(bool transient, Hsbk color, uint period, float cycles, short skewratio, byte waveform)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetWaveForm);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            packet.Payload = new WaveformPayload();
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            if(response.Error)
            {
                return new LifxCommMessage<State>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<State> finalresponse = new LifxCommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
        }

        /// <summary>
        /// Set the waveform of the light.
        /// </summary>
        /// <param name="transient">Color does not persist</param>
        /// <param name="color">Light end color</param>
        /// <param name="period">Duration of a cycle in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewratio">Waveform Skew, [-32768, 32767] scaled to [0, 1]</param>
        /// <param name="waveform">Waveform to use for transition.</param>
        /// <returns>Response</returns>
        public LifxCommMessage<State> SetWaveform(bool transient, Hsbk color, uint period, float cycles, short skewration, byte waveform)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetWaveForm);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<State>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<State> finalresponse = new LifxCommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
        }

        /// <summary>
        /// Set the waveform of the light async.
        /// </summary>
        /// <param name="transient">Color does not persist</param>
        /// <param name="color">Light end color</param>
        /// <param name="period">Duration of a cycle in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewratio">Waveform Skew, [-32768, 32767] scaled to [0, 1]</param>
        /// <param name="waveform">Waveform to use for transition.</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<State>> SetWaveformOptionalAsync(bool transient, Hsbk color, uint period, float cycles, short skewratio, byte waveform)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetWaveFormOptional);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<State>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<State> finalresponse = new LifxCommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
        }

        /// <summary>
        /// Set the waveform of the light.
        /// </summary>
        /// <param name="transient">Color does not persist</param>
        /// <param name="color">Light end color</param>
        /// <param name="period">Duration of a cycle in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewratio">Waveform Skew, [-32768, 32767] scaled to [0, 1]</param>
        /// <param name="waveform">Waveform to use for transition.</param>
        /// <returns>Response</returns>
        public LifxCommMessage<State> SetWaveformOptional(bool transient, Hsbk color, uint period, float cycles, short skewration, byte waveform)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetWaveFormOptional);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<State>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<State> finalresponse = new LifxCommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
        }

        #endregion

        #region GET_STATE
        /// <summary>
        /// Get the state of the specified light
        /// </summary>
        /// <param name="ip">IP address of the light</param>
        /// <returns>State of the light</returns>
        public async Task<LifxCommMessage<State>> GetStateAsync()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_Get);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<State>(response.Ex,null,true);
            }
            else
            {
                LifxCommMessage<State> finalresponse = new LifxCommMessage<State>(response.Ex, (State)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
           
        }
        #endregion

        #region GET_SET_INFRARED


        /// <summary>
        /// Get the current maximum power level of the infrared channel.
        /// </summary>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<StateBrightness>> GetInfraredAsync()       
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetInfrared);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<StateBrightness>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<StateBrightness> finalresponse = new LifxCommMessage<StateBrightness>(response.Ex, (StateBrightness)response.Data.data.Payload, response.Error);
                return finalresponse;
            }
        }

        /// <summary>
        /// Get the current maximum power level of the infrared channel.
        /// </summary>
        /// <returns>Response</returns>
        public LifxCommMessage<StateBrightness> GetInfrared()
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetInfrared);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            if (response.Error)
            {
                return new LifxCommMessage<StateBrightness>(response.Ex, null, true);
            }
            else
            {
                LifxCommMessage<StateBrightness> finalresponse = new LifxCommMessage<StateBrightness>(response.Ex, (StateBrightness)response.Data.data.Payload, response.Error);
                return finalresponse;
            }

        }

        /// <summary>
        /// Set the infrared level.
        /// </summary>
        /// <param name="bri">Brightness level</param>
        /// <returns>Response</returns>
        public async Task<LifxCommMessage<LifxResponse>> SetInfraredAsync(ushort bri)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetInfrared);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            packet.Payload = new StateBrightness(bri);
            LifxCommMessage<LifxResponse> response = await Lifx.SendPacketAsync(_ip, packet);
            return response;
        }

        /// <summary>
        /// Set the infrared level.
        /// </summary>
        /// <param name="bri">Brightness level</param>
        /// <returns>Response</returns>
        public LifxCommMessage<LifxResponse> SetInfrared(ushort bri)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_GetInfrared);
            packet.Header.SetTagged(false);
            packet.Header.SetTargetMAC(_mac);
            packet.Header.SetSize(packet.Header.Length);
            packet.Payload = new StateBrightness(bri);
            LifxCommMessage<LifxResponse> response = Lifx.SendPacket(_ip, packet);
            return response;
        }

        #endregion
    }
}
