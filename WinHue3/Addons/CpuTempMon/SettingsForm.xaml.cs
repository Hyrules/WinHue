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

using Action = System.Action;

namespace WinHue3
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Form_CpuTempMonitorSettings : Window
    {

        /// <summary>
        /// The Host provides a way to request information on objects present in the bridge.
        /// </summary
    

        //*********************** Local Plugin Variable *********************

        private CpuTemp Temp;

        double gradientStartColor;
        double gradientStopColor;
        double gradientStartTemp;
        double gradientStopTemp;

        public Form_CpuTempMonitorSettings(CpuTemp temp)
        {
            InitializeComponent();
            Temp = temp;
            temp.OnTempUpdated += temp_OnTempUpdated;
            Temp.Start();

            if (Dispatcher.CheckAccess())
            {
                cbListCpuSensors.Items.Clear();
                foreach (ISensor sensor in temp.cpuSensors)
                {
                    cbListCpuSensors.Items.Add(sensor);
                    if (sensor.Name.Contains("Package"))
                        cbListCpuSensors.SelectedIndex = cbListCpuSensors.Items.Count - 1;
                }
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    cbListCpuSensors.Items.Clear();
                    foreach (ISensor sensor in temp.cpuSensors)
                    {
                        cbListCpuSensors.Items.Add(sensor);
                        if (sensor.Name.Contains("Package"))
                            cbListCpuSensors.SelectedIndex = cbListCpuSensors.Items.Count - 1;
                    }

                }));

            }
        }

        private void cbListCpuSensors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() => { if (cbListCpuSensors.SelectedValue != null) { Temp.pollSensorName = cbListCpuSensors.SelectedValue.ToString(); } });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int id = -1;

            slHueGradientStart.Value = Properties.Settings.Default.CPUTemp_gradientStartColor;
            slHueGradientStop.Value = Properties.Settings.Default.CPUTemp_gradientStopColor;
            dudLowerTemp.Value = Properties.Settings.Default.CPUTemp_gradientStartTemp;
            dudUpperTemp.Value = Properties.Settings.Default.CPUTemp_gradientStopTemp;
            budBri.Value = Properties.Settings.Default.CpuTemp_Brightness;
            budSat.Value = Properties.Settings.Default.CPUTemp_Saturation;

            CommandResult bresult = BridgeStore.SelectedBridge.GetListObjects<Light>();
            CommandResult bresult2 = BridgeStore.SelectedBridge.GetListObjects<Group>();
            if (bresult.Success && bresult2.Success)
            {
                Dictionary<string, Light> lightlist = (Dictionary<string, Light>)bresult.resultobject;
                Dictionary<string, Group> grouplist = (Dictionary<string, Group>)bresult2.resultobject;

                foreach (KeyValuePair<string, Light> kvp in lightlist)
                {
                    cbObject.Items.Add(kvp);
                    if (Properties.Settings.Default.CPUTemp_ObjectType)
                    {
                        if (kvp.Key == Properties.Settings.Default.CPUTemp_ObjectID)
                            id = cbObject.Items.Count - 1;
                    }
                }

                foreach (KeyValuePair<string, Group> kvp in grouplist)
                {
                    cbObject.Items.Add(kvp);
                    if (!Properties.Settings.Default.CPUTemp_ObjectType)
                    {
                        if (kvp.Key == Properties.Settings.Default.CPUTemp_ObjectID)
                            id = cbObject.Items.Count - 1;
                    }
                }

                if (cbObject.Items.Count > 0)
                {
                    cbObject.SelectedIndex = 0;
                }

                if (id > -1)
                    cbObject.SelectedIndex = id;
            }        

        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        void temp_OnTempUpdated(object sender, CpuTempEventArgs e)
        {
            float? actualtemp = e.currentTemp;
            ushort hueTemp = 0;

            lblCpuTemp.Content = $"{Temp.temperature:0.0}ºC";
            if (!(bool)btnTestGradient.IsChecked) return;
            gradientStartColor = slHueGradientStart.Value;
            gradientStopColor = slHueGradientStop.Value;
            gradientStopTemp = (double)dudUpperTemp.Value;
            gradientStartTemp = (double)dudLowerTemp.Value;

            if ((bool)btnTestGradient.IsChecked)
            {
                if (actualtemp == null)
                {
                    hueTemp = (ushort)gradientStartColor;
                }
                else
                {

                    double gradientRange = gradientStartColor - gradientStopColor;
                    double tempRange = gradientStopTemp - gradientStartTemp;


                    if (gradientRange > 0)
                    {
                        double multiplier = gradientStartColor / tempRange;
                        hueTemp = (ushort)(gradientStartColor - (multiplier * (actualtemp - gradientStartTemp)));
                    }
                    else
                    {
                        hueTemp = (ushort)((gradientStopColor * actualtemp) / gradientStopTemp);
                    }

                }

                

                if (cbObject.SelectedItem is KeyValuePair<string, Light>)
                {
                    BridgeStore.SelectedBridge.SetState<Light>(new State() { hue = hueTemp, bri = 255, sat = 255, @on = true, transitiontime = 9 }, ((KeyValuePair<string, Light>)cbObject.SelectedItem).Key);
                }
                else
                {
                    BridgeStore.SelectedBridge.SetState<Group>(new HueLib2.Action() { hue = hueTemp, bri = 255, sat = 255, @on = true, transitiontime = 9 }, ((KeyValuePair<string, Group>)cbObject.SelectedItem).Key);
                }

            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Temp.OnTempUpdated -= temp_OnTempUpdated;
            Temp?.Stop();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CPUTemp_SensorName = ((ISensor) cbListCpuSensors.SelectedItem).Name;
            Properties.Settings.Default.CPUTemp_gradientStartColor = slHueGradientStart.Value;
            Properties.Settings.Default.CPUTemp_gradientStopColor = slHueGradientStop.Value;
            Properties.Settings.Default.CPUTemp_Saturation = (byte)budSat.Value;
            Properties.Settings.Default.CpuTemp_Brightness = (byte)budBri.Value;

            if (dudLowerTemp.Value != null)
                Properties.Settings.Default.CPUTemp_gradientStartTemp = (double)dudLowerTemp.Value;
            if (dudUpperTemp.Value != null)
                Properties.Settings.Default.CPUTemp_gradientStopTemp = (double)dudUpperTemp.Value;

            if (cbObject.SelectedItem is KeyValuePair<string, Light>)
            {
                Properties.Settings.Default.CPUTemp_ObjectType = true;
                Properties.Settings.Default.CPUTemp_ObjectID = ((KeyValuePair<string, Light>) cbObject.SelectedItem).Key;
            }
            else
            {
                Properties.Settings.Default.CPUTemp_ObjectType = false;
                Properties.Settings.Default.CPUTemp_ObjectID = ((KeyValuePair<string, Group>) cbObject.SelectedItem).Key;
            }

            Properties.Settings.Default.Save();
            DialogResult = true;
            Close();
        }

    }
}
