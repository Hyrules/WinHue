using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using WinHue3.LIFX.Payloads;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX.Responses.States.Device;

namespace WinHue3.LIFX
{
    public static class Lifx
    {      

        private static int _timeout;

        static Lifx()
        {
            _timeout = 3000;

        }

        public static int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Set the power state of all the device.
        /// </summary>
        /// <param name="level">Level between 0 and 65535</param>
        /// <param name="transitiontime">Transition time from 0 to 4,294,967,295</param>
        /// <param name="target">(Optional) Mac address of the targetted device</param>
        /// <returns>The power level from 0 to 65535</returns>
        public static async Task<CommMessage<LifxResponse>> SetPowerAllAsync(ushort level, uint transitiontime)
        {
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Light_SetPower);
            packet.Header.SetTagged(true);
            packet.Payload = new PowerPayload(level, transitiontime);
            CommMessage<LifxResponse> response = await Lifx.SendPacketAsync(IPAddress.Broadcast, packet);
            return response;
        }

        /// <summary>
        /// Get the list of LIFX devices on the network. Subscribe to event OnBroadcastComplete to get the devices.
        /// </summary>
        public static CommMessage<Dictionary<IPAddress, StateService>> GetDevices()
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout,_timeout);
            Dictionary<IPAddress, StateService> _listdevices = new Dictionary<IPAddress, StateService>();
            bool error = false;
            Exception exception = null;

            udpClient.EnableBroadcast = true;
            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetService);
            packet.Header.SetTagged(true);
            packet.Header.SetAck(false);
            packet.Header.SetResponse(false);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                udpClient.Send(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 56700));
                while (true)
                {
                    try
                    {
                        byte[] tmp = udpClient.Receive(ref endpoint);
                        Header hdr = new Header(tmp.Take(36).ToArray());
                        StateService stateservice = new StateService(hdr, tmp.Skip(36).Take(5).ToArray());
                        if (!_listdevices.ContainsKey(endpoint.Address))
                            _listdevices.Add(endpoint.Address, stateservice);

                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        error = true;
                        break;
                    }
                }
                udpClient.Close();
            }
            catch(Exception ex)
            {
                exception = ex;
                error = true;
            }
                      
            return new CommMessage<Dictionary<IPAddress, StateService>>(exception,_listdevices,error);
        }

        /// <summary>
        /// Get the list of LIFX lights asynchronously.
        /// </summary>
        /// <returns>The list of devices per IP</returns>
        public static async Task<CommMessage<Dictionary<IPAddress, StateService>>> GetDevicesAsync()
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);
            udpClient.EnableBroadcast = true;

            Dictionary<IPAddress, StateService> _listdevices = new Dictionary<IPAddress, StateService>();
            bool error = false;
            Exception exception = null;

            LifxPacket packet = new LifxPacket();
            packet.Header.SetMessageType(Header.MessageType.Device_GetService);
            packet.Header.SetTagged(true);
            packet.Header.SetAck(false);
            packet.Header.SetResponse(false);
            try
            {

                Task send = Task.Run(async () =>
                {
                    await udpClient.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 56700));
                });

                if (await Task.WhenAny(send, Task.Delay(_timeout)) != send)
                {
                    error = true;
                    exception = new TimeoutException();
                }

                if(!error)
                { 
                    Task receive = Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                UdpReceiveResult result = await udpClient.ReceiveAsync();
                                byte[] tmp = result.Buffer;
                                Header hdr = new Header(tmp.Take(36).ToArray());
                                StateService stateservice = new StateService(hdr, tmp.Skip(36).Take(5).ToArray());
                                if (!_listdevices.ContainsKey(result.RemoteEndPoint.Address))
                                    _listdevices.Add(result.RemoteEndPoint.Address, stateservice);
                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                                error = true;
                                break;
                            }

                        }

                    });

                    await Task.WhenAny(receive, Task.Delay(_timeout));

                }
            }
            catch (Exception ex)
            {
                exception = ex;
                error = true;
            }

            return new CommMessage<Dictionary<IPAddress, StateService>>(exception, _listdevices, error);
        }

        public static async Task<CommMessage<LifxResponse>> SendPacketAsync(IPAddress ip, LifxPacket packet)
        {
            UdpClient udpClient = new UdpClient();
            byte[] currentPacket = packet;
            IPEndPoint clientIp = new IPEndPoint(ip, 56700);
            LifxResponse response = new LifxResponse();
            bool error = false;
            Exception exception = null;

            try
            {
                Task send = Task.Run(async () =>
                {
                    await udpClient.SendAsync(packet, packet.Length,clientIp);
                });

                if (await Task.WhenAny(send, Task.Delay(_timeout)) != send)
                {
                    error = true;
                    exception = new TimeoutException();
                }


                if (!error)
                {
                    if (packet.Header.Ack)
                    {
                        Task receive_ack = Task.Run(async () =>
                        {
                            response.ack =
                                new LifxPacket(((UdpReceiveResult) await udpClient.ReceiveAsync()).Buffer);
                        });

                        if (await Task.WhenAny(receive_ack, Task.Delay(_timeout)) != receive_ack)
                        {
                            error = true;
                            exception = new TimeoutException();
                        }
                    }

                    if (packet.Header.Response)
                    {
                        Task receive_resp = Task.Run(async () =>
                        {
                            response.data = new LifxPacket(((UdpReceiveResult)await udpClient.ReceiveAsync()).Buffer);
                        });

                        if (await Task.WhenAny(receive_resp, Task.Delay(_timeout)) != receive_resp)
                        {
                            error = true;
                            exception = new TimeoutException();
                        }
                    }
                }

                udpClient.Close();
            }
            catch (Exception ex)
            {
                exception = ex;
                error = true;
            }

            return new CommMessage<LifxResponse>(exception,response,error);
        }

        
        public static CommMessage<LifxResponse> SendPacket(IPAddress ip, LifxPacket packet)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);

            byte[] currentPacket = packet;
            LifxResponse response = new LifxResponse();
            bool error = false;
            Exception exception = null;

            IPEndPoint clientIp = new IPEndPoint(ip ?? IPAddress.Broadcast, 56700);

            try
            {
                udpClient.Send(currentPacket, currentPacket.Length, clientIp);

                if (packet.Header.Ack)
                {
                    response.ack = new LifxPacket(udpClient.Receive(ref clientIp));
                }

                if (packet.Header.Response)
                {
                    response.data = new LifxPacket(udpClient.Receive(ref clientIp));
                }

                udpClient.Close();
            }
            catch (Exception ex)
            {
                error = true;
                exception = ex;
            }

            
            return new CommMessage<LifxResponse>(exception,response,error);
        }

   
    }

}
