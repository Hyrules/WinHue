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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ScheduleCreatorViewModel _scvm;
        private bool _isEditing = false;
        private string _id = string.Empty;

        public Form_ScheduleCreator2()
        {
            InitializeComponent();
            _scvm = DataContext as ScheduleCreatorViewModel;
            
        }

        public async Task Initialize()
        {
            await _scvm.Initialize();
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
                if (BridgeManager.BridgeManager.Instance.SelectedBridge.ModifyObject(sc))
                {
                    DialogResult = true;
                    log.Info("Schedule edition success");
                    this.Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.BridgeManager.Instance.SelectedBridge);
                }
            }
            else
            {
                if (BridgeManager.BridgeManager.Instance.SelectedBridge.CreateObject(sc))
                {
                    DialogResult = true;
                    log.Info("Schedule creation success");
                    _id = BridgeManager.BridgeManager.Instance.SelectedBridge.LastCommandMessages.LastSuccess.value;
                    this.Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.BridgeManager.Instance.SelectedBridge);
                }
            }
        }

        public string GetCreatedOrModifiedId()
        {
            return _id;
        }
    }
}
