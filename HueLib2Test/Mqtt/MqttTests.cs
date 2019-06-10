using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace WinHueTest.Mqtt
{
    [TestClass]
    public class MqttTests
    {
        IMqttClient client;

        [TestMethod]
        public async Task TestConnection()
        {
            string url = "mqtt.beebotte.com";
            MqttFactory fac = new MqttFactory();
            client = fac.CreateMqttClient();
            IMqttClientOptions options = new MqttClientOptionsBuilder().WithTcpServer(url, 1883).WithCredentials("token_4KJTOC7v1RDNLelD").Build();
            await client.ConnectAsync(options);
            Assert.IsTrue(client.IsConnected, "Client is not connected");
            await client.DisconnectAsync();
            Assert.IsFalse(client.IsConnected, "Client is not disconnected");
        }

        [TestMethod]
        public async Task TestMqttSubscription()
        {
            MqttClientSubscribeResult res = await client.SubscribeAsync("gh/tv");
            
        }
    }
}
