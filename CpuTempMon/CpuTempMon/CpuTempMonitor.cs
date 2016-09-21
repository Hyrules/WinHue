using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinHuePluginModule;
using HueLib2;

namespace CpuTempMon
{
    [Export(typeof(IWinHuePluginModule))]
    class CpuTempMonitor : IWinHuePluginModule
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        const string PluginName = "Cpu Temp Monitor";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        const string PluginDesc = "Adjust a light or group of light depending on the temperature of your cpu.";

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        const string PluginAuth = "Pascal Pharand";

        CpuTemp _temp;

#pragma warning disable 649       
        [Import(typeof(IWinhuePluginHost))]
        public IWinhuePluginHost Host;
#pragma warning restore 649

        double _gradientStartColor;
        double _gradientStopColor;
        double _gradientStopTemp;
        double _gradientStartTemp;
        string _objectId;
        bool _objectType;
        byte _userBri;
        byte _userSat;

        [ImportingConstructor]
        public CpuTempMonitor()
        {
            _temp = new CpuTemp(1);
            _temp.OnTempUpdated += temp_OnTempUpdated;
            LoadSettings();
        }

        private void LoadSettings()
        {
            _gradientStartColor = Properties.Settings.Default.CPUTemp_gradientStartColor;
            _gradientStartTemp = Properties.Settings.Default.CPUTemp_gradientStartTemp;
            _gradientStopColor = Properties.Settings.Default.CPUTemp_gradientStopColor;
            _gradientStopTemp = Properties.Settings.Default.CPUTemp_gradientStopTemp;
            _objectId = Properties.Settings.Default.CPUTemp_ObjectID;
            _objectType = Properties.Settings.Default.CPUTemp_ObjectType;
            _userBri = Properties.Settings.Default.CpuTemp_Brightness;
            _userSat = Properties.Settings.Default.CPUTemp_Saturation;

        }

        /// <summary>
        /// Required for the interface to know what the plugin name is.
        /// </summary>
        public string pluginName
        {
            get
            {
                return PluginName;
            }
        }

        /// <summary>
        /// Required for the interface to know what the plugin description is.
        /// </summary>
        public string pluginDesc
        {
            get
            {
                return PluginDesc;
            }
        }

        /// <summary>
        /// Required for the interface to know what the plugin author is.
        /// </summary>
        public string pluginAuth
        {
            get
            {
                return PluginAuth;
            }

        }

        /// <summary>
        /// Required by the interface get the icon image.
        /// </summary>
        public Bitmap pluginIcon
        {
            get
            {
                return Properties.Resources.pluginicon;
            }
        }

        /// <summary>
        /// Do the plugin work.
        /// </summary>
        public void Start()
        {
            _temp.Start();
        }

        /// <summary>
        /// Stop the plugin.
        /// </summary>
        public void Stop()
        {
            _temp.Stop();
        }

        /// <summary>
        /// Show the settings form for the plugin.
        /// </summary>
        /// <returns>True or false or null depending on what you want to return.</returns>
        public bool? ShowSettingsForm()
        {
            _temp.Stop();
            Form_CpuTempMonitorSettings settings = new Form_CpuTempMonitorSettings(_temp, Host) {Owner = Application.Current.MainWindow};
            _temp.OnTempUpdated -= temp_OnTempUpdated;
            var result = settings.ShowDialog();
            _temp.OnTempUpdated += temp_OnTempUpdated;
            if (result == true)
                LoadSettings();
            return result;
        }
        
        void temp_OnTempUpdated(object sender, CpuTempEventArgs e)
        {
            float? actualtemp = e.currentTemp;
            ushort hueTemp = 0;

            if (actualtemp == null)
            {
                hueTemp = (ushort)_gradientStartColor;
            }
            else
            {

                double gradientRange = _gradientStartColor - _gradientStopColor;
                double tempRange = _gradientStopTemp - _gradientStartTemp;

                // Check for the order of the sliders. Is the first slider before the 2nd or the other way around.
                if (gradientRange > 0)
                {
                    double multiplier = _gradientStartColor / tempRange;
                    hueTemp = (ushort)(_gradientStartColor - (multiplier * (actualtemp - _gradientStartTemp)));
                }
                else
                {
                    hueTemp = (ushort)((_gradientStopColor * actualtemp) / _gradientStopTemp);
                }

            }

            if (_objectType == true)
            {
                Host.SetLightState(_objectId, new State() { hue = hueTemp, bri = _userBri, sat = _userSat, @on = true, transitiontime = 9 });
            }
            else
            {
                Host.SetGroupState(_objectId, new HueLib2.Action() { hue = hueTemp, bri = _userBri, sat = _userSat, @on = true, transitiontime = 9 });
            }

          }
    }
}
