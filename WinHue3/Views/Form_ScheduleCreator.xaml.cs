using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using HueLib2;
using HueLib2.Objects.HueObject;
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
        private IHueObject actualobj;

        public Form_ScheduleCreator(IHueObject obj, Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            scvm = this.DataContext as ScheduleCreatorViewModel;
            scvm.Initialize(bridge);
   
           
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

                scvm.SelectedObject = scvm.ListTarget.FirstOrDefault(x => x.Id == obj.Id && x.GetType() == obj.GetType());

            }

            Title = obj is Schedule ? GUI.ScheduleCreatorForm_Title_Modify + obj.Name : GUI.ScheduleCreatorForm_Title_Create + obj.Name;
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Schedule sc = scvm.Schedule;
            CommandResult<MessageCollection> comres;
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
                MessageCollection mc = comres.Data;
                actualobj = new Schedule() { Id = mc[0].ToString()};
                Close();
            }
            else
            {
                MessageBox.Show($"{GlobalStrings.Error_ErrorHasOccured} : {_bridge.lastMessages}", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }    

        }

        public string GetCreatedOrModifiedID()
        {
            return actualobj.Id;
        }
    }
}
