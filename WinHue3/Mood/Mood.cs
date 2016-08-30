using System;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Moods.
    /// </summary>
    [Serializable()]
    public class Mood
    {
        /// <summary>
        /// Name of the mood.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the mood.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// On.
        /// </summary>
        public bool? on { get; set; }
        /// <summary>
        /// Brightness
        /// </summary>
        public byte? bri { get; set; }
        /// <summary>
        /// Color
        /// </summary>
        public ushort? hue { get; set; }
        /// <summary>
        /// Saturation
        /// </summary>
        public byte? sat { get; set; }
        /// <summary>
        /// Color coordinates in XY
        /// </summary>
        public XY xy;
        /// <summary>
        /// Color temperature.
        /// </summary>
        public ushort? ct { get; set; }
        /// <summary>
        /// Alert
        /// </summary>
        public string alert { get; set; }
        /// <summary>
        /// Effect
        /// </summary>
        public string effect { get; set; }
        /// <summary>
        /// Color mode
        /// </summary>
        public string colormode { get; set; }
        /// <summary>
        /// Transition time.
        /// </summary>
        public ushort? transitiontime { get; set; } // TransitionTime
    }
}