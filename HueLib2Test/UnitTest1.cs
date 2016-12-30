using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HueLib2;
using WinHue3;
using WinHue3.ViewModels;

namespace HueLib2Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string data =
                "{\"lights\":{\"1\":{\"state\":{\"on\":false,\"bri\":124,\"alert\":\"none\",\"reachable\":true},\"type\":\"Dimmable light\",\"name\":\"Hue white lamp 1\",\"modelid\":\"LWB006\",\"manufacturername\":\"Philips\",\"uniqueid\":\"00:17:88:01:10:2f:3d:29-0b\",\"swversion\":\"66015095\"},\"2\":{\"state\":{\"on\":false,\"bri\":1,\"alert\":\"none\",\"reachable\":true},\"type\":\"Dimmable light\",\"name\":\"Hue white lamp 2\",\"modelid\":\"LWB006\",\"manufacturername\":\"Philips\",\"uniqueid\":\"00:17:88:01:10:2c:d3:db-0b\",\"swversion\":\"66015095\"}},\"groups\":{},\"config\":{\"name\":\"Philips hue\",\"zigbeechannel\":15,\"bridgeid\":\"001788FFFE2B8C5E\",\"mac\":\"00:17:88:2b:8c:5e\",\"dhcp\":true,\"ipaddress\":\"192.168.1.35\",\"netmask\":\"255.255.255.0\",\"gateway\":\"192.168.1.1\",\"proxyaddress\":\"none\",\"proxyport\":0,\"UTC\":\"2016-12-29T19:58:57\",\"localtime\":\"2016-12-29T20:58:57\",\"timezone\":\"Europe/Zurich\",\"modelid\":\"BSB002\",\"swversion\":\"01031131\",\"apiversion\":\"1.12.0\",\"swupdate\":{\"updatestate\":0,\"checkforupdate\":false,\"devicetypes\":{\"bridge\":false,\"lights\":[],\"sensors\":[]},\"url\":\"\",\"text\":\"\",\"notify\":false},\"linkbutton\":false,\"portalservices\":false,\"portalconnection\":\"connected\",\"portalstate\":{\"signedon\":true,\"incoming\":false,\"outgoing\":true,\"communication\":\"disconnected\"},\"factorynew\":false,\"replacesbridgeid\":null,\"backup\":{\"status\":\"idle\",\"errorcode\":0},\"whitelist\":{\"UQ6W3eZhI-WiVW5fGnRkvYydvy3O-4fFsdLfuf5N\":{\"last use date\":\"2016-12-28T15:44:20\",\"create date\":\"2016-12-28T15:18:46\",\"name\":\"WinHue\"},\"6ZTRDH7PFr2-oneEm0u2GfSTRqlu8mxq6E-VwcGt\":{\"last use date\":\"2016-12-28T16:29:20\",\"create date\":\"2016-12-28T15:37:37\",\"name\":\"JPV7\"},\"63fclkHJJbQ2qwKzu5FUU2aInAqbgTWktXq8-d2R\":{\"last use date\":\"2016-12-28T21:25:33\",\"create date\":\"2016-12-28T16:37:40\",\"name\":\"WinHue\"},\"Yb0I23XGdUCn8TH8tbq8CIAgWxCKXQOAxmciRQNP\":{\"last use date\":\"2016-12-29T08:25:33\",\"create date\":\"2016-12-28T21:14:23\",\"name\":\"JPVW7\"},\"JEzVHio2zyc519M6zRoui3VpRyzNA2EnfH9SHX3i\":{\"last use date\":\"2016-12-28T21:40:33\",\"create date\":\"2016-12-28T21:20:05\",\"name\":\"Huetro#Windows.Desktop\"},\"NStGKx5J4ZAvPDlUcubpYledjbSGqElKq1yhCbRN\":{\"last use date\":\"2016-12-29T19:58:57\",\"create date\":\"2016-12-29T10:31:09\",\"name\":\"my_hue_app#JPVW7\"}}},\"schedules\":{\"1\":{\"name\":\"Test_on\",\"description\":\"Hue white lamp 1\",\"command\":{\"address\":\"/api/JEzVHio2zyc519M6zRoui3VpRyzNA2EnfH9SHX3i/lights/1/state\",\"body\":{\"bri\":124,\"on\":true},\"method\":\"PUT\"},\"localtime\":\"2016-12-28T22:51:00\",\"time\":\"2016-12-28T21:51:00\",\"created\":\"2016-12-28T21:50:01\",\"status\":\"disabled\",\"autodelete\":false,\"recycle\":false}},\"scenes\":{},\"rules\":{},\"sensors\":{\"1\":{\"state\":{\"daylight\":false,\"lastupdated\":\"2016-12-29T14:51:00\"},\"config\":{\"on\":true,\"long\":\"15.0000E\",\"lat\":\"46.5338N\",\"sunriseoffset\":30,\"sunsetoffset\":-30},\"name\":\"Daylight\",\"type\":\"Daylight\",\"modelid\":\"PHDL00\",\"manufacturername\":\"Philips\",\"swversion\":\"1.0\"},\"2\":{\"state\":{\"temperature\":1827,\"lastupdated\":\"2016-12-29T19:58:27\"},\"config\":{\"on\":true,\"battery\":100,\"reachable\":true},\"name\":\"Thermostat 1\",\"type\":\"ZLLTemperature\",\"modelid\":\"SML001\",\"manufacturername\":\"Philips\",\"swversion\":\"6.1.0.18912\",\"uniqueid\":\"00:17:88:01:02:02:07:4c-02-0402\"},\"3\":{\"state\":{\"presence\":false,\"lastupdated\":\"2016-12-29T19:55:41\"},\"config\":{\"on\":true,\"battery\":100,\"reachable\":true},\"name\":\"Occupancy Sensor 1\",\"type\":\"ZLLPresence\",\"modelid\":\"SML001\",\"manufacturername\":\"Philips\",\"swversion\":\"6.1.0.18912\",\"uniqueid\":\"00:17:88:01:02:02:07:4c-02-0406\"}},\"resourcelinks\":{}}";
            DataStore test = Serializer.DeserializeToObject<DataStore>(data);

            //ScheduleCreatorViewModel sv = new ScheduleCreatorViewModel();

        }
    }
}
