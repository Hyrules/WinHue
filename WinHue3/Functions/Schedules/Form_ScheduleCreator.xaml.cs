using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Resources;
using WinHue3.ViewModels;


namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for ScheduleCreator.xaml
    /// </summary>
    public partial class Form_ScheduleCreator : Window
    {
        private ScheduleCreatorViewModel scvm;
        private Bridge _bridge;
        private IHueObject actualobj;


        public Form_ScheduleCreator()
        {
            InitializeComponent();
            scvm = this.DataContext as ScheduleCreatorViewModel;
        
        }


        public async Task Initialize(Bridge bridge, IHueObject obj)
        {
            _bridge = bridge;
            await scvm.Initialize(bridge, obj);

            actualobj = obj;

            Title = obj is Schedule ? GUI.ScheduleCreatorForm_Title_Modify + obj.name : GUI.ScheduleCreatorForm_Title_Create + obj.name;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = scvm.Schedule;
            bool result;
            if (actualobj is Schedule)
            {
                sc.Id = actualobj.Id;
                result = _bridge.ModifyObject(sc);             
            }
            else
            {
                result = _bridge.CreateObject(sc);
            }

            if (result)
            {
                DialogResult = true;
                actualobj = new Schedule() { Id = _bridge.LastCommandMessages.LastSuccess.value};
                Close();
            }
            else
            {
                MessageBox.Show($"{GlobalStrings.Error_ErrorHasOccured} : {_bridge.LastCommandMessages}", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }    

        }

        public string GetCreatedOrModifiedID()
        {
            return actualobj.Id;
        }
    }
}
