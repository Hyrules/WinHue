using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HueLib2;

namespace WinHue3
{
    
    public class CpuTempMonitor : View
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        private const string PluginName = "Cpu Temp Monitor";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        private const string PluginDesc = "Adjust a light or group of light depending on the temperature of your cpu.";

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        private const string PluginAuth = "Pascal Pharand";

        CpuTemp _temp;

        double _gradientStartColor;
        double _gradientStopColor;
        double _gradientStopTemp;
        double _gradientStartTemp;
        string _objectId;
        bool _objectType;
        byte _userBri;
        byte _userSat;
        private bool _isrunning;
        private readonly Bridge _bridge;

        public CpuTempMonitor(Bridge bridge)
        {
            _bridge = bridge;
            Temp = new CpuTemp(1);
            Temp.OnTempUpdated += temp_OnTempUpdated;
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

        public bool IsRunning
        {
            get { return _isrunning;}
            private set { _isrunning = value; RaisePropertyChanged(); }

        }

        /// <summary>
        /// Required for the interface to know what the plugin name is.
        /// </summary>
        public string pluginName => PluginName;

        /// <summary>
        /// Required for the interface to know what the plugin description is.
        /// </summary>
        public string pluginDesc => PluginDesc;

        /// <summary>
        /// Required for the interface to know what the plugin author is.
        /// </summary>
        public string pluginAuth => PluginAuth;

        /// <summary>
        /// Required by the interface get the icon image.
        /// </summary>
        public Bitmap pluginIcon => Properties.Resources.cputemp;

        public CpuTemp Temp
        {
            get
            {
                return _temp;
            }

            set
            {
                _temp = value;
            }
        }

        /// <summary>
        /// Do the plugin work.
        /// </summary>
        public void Start()
        {
            Temp.Start();
            IsRunning = true;
        }

        /// <summary>
        /// Stop the plugin.
        /// </summary>
        public void Stop()
        {
            Temp.Stop();
            IsRunning = false;
        }

        /// <summary>
        /// Show the settings form for the plugin.
        /// </summary>
        /// <returns>True or false or null depending on what you want to return.</returns>
        public bool? ShowSettingsForm()
        {
            Temp.Stop();
            Form_CpuTempMonitorSettings settings = new Form_CpuTempMonitorSettings(Temp,_bridge) {Owner = Application.Current.MainWindow};
            Temp.OnTempUpdated -= temp_OnTempUpdated;
            var result = settings.ShowDialog();
            Temp.OnTempUpdated += temp_OnTempUpdated;
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
                _bridge.SetState<Light>(new State() { hue = hueTemp, bri = _userBri, sat = _userSat, @on = true, transitiontime = 9 },_objectId);
            }
            else
            {
                _bridge.SetState<Group>(new HueLib2.Action() { hue = hueTemp, bri = _userBri, sat = _userSat, @on = true, transitiontime = 9 },_objectId);
            }

          }
    }
}
