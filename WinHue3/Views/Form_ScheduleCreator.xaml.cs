using System.Windows;
using HueLib2;
using WinHue3.Resources;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for ScheduleCreator.xaml
    /// </summary>
    public partial class Form_ScheduleCreator : Window
    {
        private ScheduleCreatorViewModel scvm;
        private readonly Bridge _bridge;
        private HueObject actualobj;

        public Form_ScheduleCreator(HueObject obj, Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            scvm = this.DataContext as ScheduleCreatorViewModel;
            string id = string.Empty;
           
            actualobj = obj;

            if (obj is Schedule)
            {
                scvm.Schedule = (Schedule)obj;
            }
            else
            {
                if (obj is Scene)
                {
                    scvm.CanSetSettings = false;
                    scvm.IsEditing = true;
                    scvm.ScheduleModel.Scene = ((Scene) obj).Id;
                }
                scvm.TargetObject = $@"/api/{_bridge.ApiKey}/{(obj is Light ? "lights" : "groups")}/{(obj is Scene ? "0": obj.Id)}/{(obj is Light ? "state" : "action")}";
            }

            Title = obj is Schedule ? GUI.ScheduleCreatorForm_Title_Modify + obj.GetName() : GUI.ScheduleCreatorForm_Title_Create + obj.GetName();
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = scvm.Schedule;
            CommandResult comres;
            if (actualobj is Schedule)
            {
                comres = _bridge.ModifyObject<Schedule>(sc, actualobj.Id);
                
            }
            else
            {
                comres = _bridge.CreateObject<Schedule>(sc);
            }

            if (comres.Success)
            {
                DialogResult = true;
                MessageCollection mc = (MessageCollection) comres.resultobject;
                actualobj = new Schedule() { Id = mc[0].ToString()};
                Close();
            }
            else
            {
                MessageBox.Show($"{GlobalStrings.Error_ErrorHasOccured} : {_bridge.lastMessages.ToString()}", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }    

        }

        public string GetCreatedOrModifiedID()
        {
            return actualobj.Id;
        }
    }
}
