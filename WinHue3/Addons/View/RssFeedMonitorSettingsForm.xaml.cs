using System.Windows;
using WinHue3.Addons.ViewModel;

namespace WinHue3.Addons.View
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
