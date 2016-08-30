using System.Windows;
using HueLib2;
using WinHue3.Resources;
namespace WinHue3
{
    /// <summary>
    /// Interaction logic for ScheduleCreator.xaml
    /// </summary>
    public partial class Form_ScheduleCreator : Window
    {
        /// <summary>
        /// Current bridge.
        /// </summary>
        readonly Bridge _bridge;

        private string _id;

        private ScheduleView _scheduleMv;

        private readonly HueObject actualobj;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="br">Current bridge.</param>
        /// <param name="obj">object being scheduled.</param>
        public Form_ScheduleCreator(Bridge br, HueObject obj)
        {
            
            _scheduleMv = new ScheduleView(obj,br.ApiKey);

            DataContext = _scheduleMv;
            InitializeComponent();
            
            _bridge = br;
            actualobj = obj;

            Title = obj is Schedule ? GUI.ScheduleCreatorForm_Title_Modify + obj.GetName() : GUI.ScheduleCreatorForm_Title_Create + obj.GetName();
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = _scheduleMv.GetSchedule();
            if (actualobj is Schedule)
            {
                string id = sc.Id;
                sc.Id = null;
                sc.created = null;
                sc.recycle = null;
                _bridge.UpdateSchedule(id, sc);
            }
            else
            {
                _id = _bridge.CreateSchedule(sc);
            }

            if (_bridge.lastMessages.SuccessCount > 0 && _bridge.lastMessages.FailureCount == 0)
            {
                DialogResult = true;
                if (actualobj is Schedule) _id = actualobj.Id;
                Close();
            }
            else
            {
                MessageBox.Show($"{GlobalStrings.Error_ErrorHasOccured} : {_bridge.lastMessages.ToString()}", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }    

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }
    }
}
