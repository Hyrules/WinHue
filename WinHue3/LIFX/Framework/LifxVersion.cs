using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Annotations;
using WinHue3.LIFX.Framework.Responses.States.Device;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.LIFX.Framework
{
    public static class LifxVendors
    {
        private static List<LifxVendor> _vendors;

        static LifxVendors()
        {
            _vendors = Serializer.DeserializeToObject<List<LifxVendor>>(Properties.Resources.productid);
        }

        public static LifxProduct GetProduct(int vid, int pid)
        {
            LifxVendor v = _vendors.Find(x => x.vid == vid);
            return  v?.products.Find(x => x.pid == pid);
        }

        public static LifxVendor GetVendor(int vid)
        {
            return _vendors.Find(x => x.vid == vid);
        }
    }

    public class LifxFeatures
    {
        public bool color { get; set; }
        public bool infrared { get; set; }
        public bool multizone { get; set; }
        public bool chain { get; set; }
    }

    public class LifxProduct
    {
        public int pid { get; set; }
        public string name { get; set; }
        public LifxFeatures features { get; set; }
    }

    public class LifxVendor
    {
        public int vid { get; set; }
        public string name { get; set; }
        public List<LifxProduct> products { get; set; }
    }
}
