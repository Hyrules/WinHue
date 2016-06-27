using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace WinhueLightPlugin
{

    /// <summary>
    /// Interface for lights module.
    /// </summary>
    [ComImport]
    [Guid("38B9F7CE-1C58-47B2-A892-103F0D637B63")]
    public interface IWinHueLightModule
    {
        /// <summary>
        /// Name of the Light that will be displayed.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Type of the light.
        /// </summary>
        string type { get; }

        /// <summary>
        /// ModelId of the light.
        /// </summary>
        string modelid { get; }

        /// <summary>
        /// The 3 small icon ON OFF UNR that are needed for Winhue. (32x32px)
        /// </summary>
        Dictionary<string, ImageSource> IconSmall { get; }

        /// <summary>
        /// The 3 large icon ON OFF UNR that are needed for Winhue. (64x64px)
        /// </summary>
        Dictionary<string, ImageSource> IconLarge { get; }

        /// <summary>
        /// The permission of the lights. ON, CT, EFFECT, ALERT...
        /// </summary>
        Dictionary<string, bool> permissions { get; }

    }
}
