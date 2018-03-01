using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgePairing;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    /// <summary>
    /// Interaction logic for Form_ScheduleCreator2.xaml
    /// </summary>
    public partial class Form_ScheduleCreator2 : Window
    {
        public ScheduleCreatorViewModel _scvm;
        private Bridge _bridge;
        private bool _isEditing = false;
        private string _id = string.Empty;

        public Form_ScheduleCreator2()
        {
            InitializeComponent();
            _scvm = DataContext as ScheduleCreatorViewModel;
            
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            await _scvm.Initialize(_bridge);
        }

        public void EditSchedule(Schedule sc)
        {
            _isEditing = true;
            _id = sc.Id;
            _scvm.EditSchedule(sc);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = _scvm.GetSchedule();
            if (_isEditing)
            {
                sc.Id = _id;
                if (_bridge.ModifyObject(sc))
                {
                    this.Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }
            else
            {
                if (_bridge.CreateObject(sc))
                {
                    this.Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }
        }
    }
}
