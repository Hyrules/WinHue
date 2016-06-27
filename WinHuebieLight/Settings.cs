using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHuebieLight
{
    enum ScanMethod
    {
        Avarage = 1,
        HueBoost = 2,
        LumaOnly = 3
    };
    static class Settings
    {
        /// <summary>
        /// List of lights to use in the setter
        /// </summary>
        public static List<int> lights = new List<int>();
        public static List<int> groups = new List<int>();
        /// <summary>
        /// Saturation boost/ reduction. 1.0 is 100%
        /// </summary>
        public static float saturation = 1.0f;
        /// <summary>
        /// Luminosity boost/ reduction. 1.0 is 100%
        /// </summary>
        public static float luminuosity = 1.0f;
        /// <summary>
        /// Size reduction for the scan image. 1=100% = fuullsize scan.
        /// </summary>
        public static float scanQuality = 0.05f;
        /// <summary>
        /// Method for scanner
        /// </summary>
        public static ScanMethod scanMethod = ScanMethod.Avarage;
        /// <summary>
        /// transition time for lighting changes
        /// </summary>
        public static ushort transitiontime = 3;
        /// <summary>
        /// minimum light even in pitch black scenes
        /// </summary>
        public static byte minLight = 20;
        /// <summary>
        /// maximum light level, even in brightest scenes
        /// </summary>
        public static byte maxLight = 250;

        /// <summary>
        /// minimum change that is required to send new lighting
        /// state. Used to prevent spamming the bridge with lighting 
        /// requests for minimal changes.
        /// </summary>
        public static float minDelta = 0.1f;
    }

}
