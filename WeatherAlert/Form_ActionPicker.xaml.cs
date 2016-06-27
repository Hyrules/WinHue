using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HueLib_base;
using WinHuePluginModule;
using Color = System.Drawing.Color;

namespace RssFeedMonitor
{
    /// <summary>
    /// Interaction logic for Form_ActionPicker.xaml
    /// </summary>
    public partial class Form_ActionPicker : Window
    {
        private IWinhuePluginHost _host;
        private List<string> _listselectedLights;
        private Body _action;

        public Form_ActionPicker(IWinhuePluginHost host)
        {
            InitializeComponent();
            _host = host;
            _action = null;
            _listselectedLights = null;
        }

        public Form_ActionPicker(IWinhuePluginHost host, Body action, List<string> listlights )
        {
            InitializeComponent();
            _host = host;
            _action = action;
            _listselectedLights = listlights;
        }


        private void slHue_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slSATEndGradient == null) return;
            double val = slHue.Value / 273.06;
            Color color = new HSLColor(val, 240, 120);
            slSATEndGradient.Color = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        private void ChbHue_OnChecked(object sender, RoutedEventArgs e)
        {
            chbCT.IsChecked = false;
            chbX.IsChecked = false;
        }

        private void ChbCT_OnChecked(object sender, RoutedEventArgs e)
        {
            chbHue.IsChecked = false;
            chbX.IsChecked = false;
        }

        private void ChbX_OnChecked(object sender, RoutedEventArgs e)
        {
            chbHue.IsChecked = false;
            chbCT.IsChecked = false;

        }

        private void SlTT_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slTT.Value >= 0)
            {
                int time = (int)(slTT.Value * 100);
                if (time == 0)
                {
                    lblTT.Content = "Transition Time : Instantaneous";
                }
                else if (time > 0 && time < 1000)
                {
                    lblTT.Content = $"Transition Time : {(double)time:0.##} ms";
                }
                else if (time >= 1000 && time < 60000)
                {
                    lblTT.Content = $"Transition Time : {((double)time / 1000):0.##} seconds";
                }
                else if (time >= 60000)
                {
                    lblTT.Content = $"Transition Time : {((double)time / 60000):0.##} minutes";
                }

            }
            else
            {
                if (lblTT != null)
                    lblTT.Content = "Transition Time : none";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dictionary<string, Light> listlights = _host.GetLightList();

            foreach (KeyValuePair<string, Light> kvp in listlights)
            {
                lbLights.Items.Add(kvp);
            }

            if (_action == null) return;

            if (_action.hue != null)
            {
                chbHue.IsChecked = true;
                slHue.Value = (double)_action.hue;
            }

            if (_action.bri != null)
            {
                chbBri.IsChecked = true;
                slBri.Value = (double) _action.bri;
            }

            if (_action.sat != null)
            {
                chbSAT.IsChecked = true;
                slSAT.Value = (double) _action.sat;
            }

            if (_action.ct != null)
            {
                chbCT.IsChecked = true;
                slCT.Value = (double) _action.ct;
            }

            if (_action.xy != null)
            {
                chbX.IsChecked = true;
             //   slX.Value = (double) _action.xy[0];
             //   slY.Value = (double) _action.xy[1];
            }

            if (_action.transitiontime != null)
            {
                chbTT.IsChecked = true;
                slTT.Value = (double) _action.transitiontime;
            }

            if (_action.alert != null)
            {
                switch (_action.alert)
                {
                    case "none":
                        cbAlert.SelectedIndex = 0;
                        break;
                    case "select":
                        cbAlert.SelectedItem = 1;
                        break;
                    case "lselect":
                        cbAlert.SelectedItem = 2;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                cbAlert.SelectedIndex = 0;
            }

            if (_listselectedLights == null) return;

            foreach (KeyValuePair<byte, Light> kvp in lbLights.Items.Cast<KeyValuePair<byte, Light>>().Where(kvp => _listselectedLights.Contains(kvp.Key.ToString())))
            {
                lbLights.SelectedItems.Add(kvp);
            }

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {

            if ((!(bool)chbHue.IsChecked && !(bool)chbBri.IsChecked && !(bool)chbSAT.IsChecked && !(bool)chbCT.IsChecked && !(bool)chbX.IsChecked && cbAlert.SelectedIndex == 0) && (bool)chbOn.IsChecked)
            {
                MessageBox.Show("Please check at least one action to do when the conditions are met.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lbLights.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please check at least one light on which to apply the action when conditions are met.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _action = new Body();
            _listselectedLights = new List<string>();

            if ((bool)chbHue.IsChecked)
                _action.hue = (byte)slHue.Value;

            if ((bool)chbBri.IsChecked)
                _action.bri = (byte)slBri.Value;

            if ((bool)chbCT.IsChecked)
                _action.ct = (ushort)slCT.Value;

            if ((bool)chbSAT.IsChecked)
                _action.sat = (byte)slSAT.Value;

            if ((bool) chbX.IsChecked)
            {
                _action.xy = new XY();
                _action.xy.x = (decimal) slX.Value;
                _action.xy.y = (decimal) slY.Value;
            }
            if ((bool)chbTT.IsChecked)
                _action.transitiontime = (ushort)slTT.Value;

            _action.on = chbOn.IsChecked;

            switch (cbAlert.SelectedIndex)
            {
                case 0:
                    _action.alert = null;
                    break;
                case 1:
                    _action.alert = "select";
                    break;
                case 2:
                    _action.alert = "lselect";
                    break;
                default:
                    _action.alert = null;
                    break;
            }

            
            foreach (KeyValuePair<byte, Light> kvp in lbLights.SelectedItems)
            {
                _listselectedLights.Add(kvp.Key.ToString());
            }

            DialogResult = true;
            Close();

        }

        public List<string> GetSelectedLights()
        {
            return _listselectedLights;
        }

        public Body GetAction()
        {
            return _action;
        }

        public void ClearAction()
        {
            _listselectedLights = new List<string>();
            _action = new Body();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
