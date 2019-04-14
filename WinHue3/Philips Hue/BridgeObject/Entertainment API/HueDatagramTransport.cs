using System;
using System.Net.Sockets;
using Org.BouncyCastle.Crypto.Tls;

namespace WinHue3.Philips_Hue.BridgeObject.Entertainment_API
{
    public class HueDatagramTransport : DatagramTransport
    {

        private readonly Socket socket;

        public HueDatagramTransport(Socket socket)
        {
            this.socket = socket;
        }

        public int GetReceiveLimit()
        {
            return 4096;
        }

        public int GetSendLimit()
        {
            return 4096;
        }

        public int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            Console.Write(buf);
            return socket.Receive(buf, off, len, SocketFlags.None);
        }

        public void Send(byte[] buf, int off, int len)
        {
            Console.Write(buf);
            socket.Send(buf, off, len, SocketFlags.None);
                
        }

        public void Close()
        {
            socket.Close();
        }
    }
}
