using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xaml;
using System.Xml;
using WinHue3.Addons.ViewModel;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for WeatherSettingsForm.xaml
    /// </summary>
    public partial class RssFeedMonitorSettingsForm : Window
    {
        private RssFeedSettingsViewModel rfmsv;

        public RssFeedMonitorSettingsForm()
        {
            InitializeComponent();
            rfmsv = DataContext as RssFeedSettingsViewModel;

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            rfmsv.SaveSettings();
            Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
