using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgePairing;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Functions.Schedules.NewCreator
{
    /// <summary>
    /// Interaction logic for Form_ScheduleCreator2.xaml
    /// </summary>
    public partial class Form_ScheduleCreator2 : Window
    {
        public ScheduleCreatorViewModel _scvm;
        private Bridge _bridge;

        public Form_ScheduleCreator2(Bridge bridge)
        {
            InitializeComponent();
            _scvm = DataContext as ScheduleCreatorViewModel;
            _bridge = bridge;
        }

        public async Task Initialize()
        {
            await _scvm.Initialize(_bridge);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = _scvm.GetSchedule();
        }
    }
}
