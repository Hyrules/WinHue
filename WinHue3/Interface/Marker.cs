using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace WinHue3.Interface
{
    /// <summary>
    /// Marker on the scene creator window.
    /// </summary>
    [DataContract, Serializable]
    public class Marker
    {
        /// <summary>
        /// Position of the maker in the picture box
        /// </summary>
        public Point PositionPictureBox;

        /// <summary>
        /// Position of the marker in the image
        /// </summary>
        public Point PositionImage;

        /// <summary>
        /// Color of the marker.
        /// </summary>
        public int color { get; set; }

        /// <summary>
        /// Id of the light or group
        /// </summary>
        public byte id { get; set; }

        /// <summary>
        /// Name of the marker.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Saturation of the marker.
        /// </summary>
        public byte sat { get; set; }

        /// <summary>
        /// Brightness of the marker.
        /// </summary>
        public byte bri { get; set; } // Brightness

        /// <summary>
        /// Hue of the marker.
        /// </summary>
        public ushort hue { get; set; } // Hue Color
    }
}