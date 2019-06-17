using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Functions.Application_Settings.Settings;

namespace WinHue3.Philips_Hue.Communication2
{
    public static class Communication2
    {

        private static HttpClient client;

        static Communication2()
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, WinHueSettings.settings.Timeout);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}
