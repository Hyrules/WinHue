using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HueLib2;
using Action = HueLib2.Action;
using System.Collections.Generic;
using HueLib2.Objects.Rules;
using HueLib2.BridgeMessages;

namespace HueLib2Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Messages msg = Serializer.DeserializeToObject<Messages>("[ { \"success\": { \" / lights\": \"Searching for new devices\" } }, { \"error\": { \"type\": 123 , \"address\": \" resource / parameteraddress \" , \"description\": \"description\"  }} ]");
        }
    }
}
