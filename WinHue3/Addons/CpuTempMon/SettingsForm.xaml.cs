using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HueLib2;
using OpenHardwareMonitor.Hardware;
using WinHue3.Addons.CpuTempMon;
using Action = System.Action;

namespace WinHue3
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Form_CpuTempMonitorSettings : Window
    {
        private CpuTempViewModel _cpuvm;
        /// <summary>
        /// The Host provides a way to request information on objects present in the bridge.
        /// </summary
    

        //*********************** Local Plugin Variable *********************

        public Form_CpuTempMonitorSettings(CpuTemp temp,Bridge bridge)
        {
            InitializeComponent();
            _cpuvm = DataContext as CpuTempViewModel;
            _cpuvm.Initialize(bridge, temp);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   

        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _cpuvm.Temp?.Stop();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CPUTemp_SensorName = _cpuvm.SelectedSensor.Name;
            Properties.Settings.Default.CPUTemp_gradientStartColor = slHueGradientStart.Value;
            Properties.Settings.Default.CPUTemp_gradientStopColor = slHueGradientStop.Value;
            Properties.Settings.Default.CPUTemp_Saturation = _cpuvm.Sat;
            Properties.Settings.Default.CpuTemp_Brightness = _cpuvm.Bri;

            Properties.Settings.Default.CPUTemp_gradientStartTemp = _cpuvm.LowerTemp;
            Properties.Settings.Default.CPUTemp_gradientStopTemp = _cpuvm.UpperTemp;
            Properties.Settings.Default.CPUTemp_ObjectID = _cpuvm.SelectedObject.Id;

            Properties.Settings.Default.Save();
            DialogResult = true;
            Close();
        }

    }
}
