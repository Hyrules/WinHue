using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HueLib2;
using HueLib2.BridgeMessages;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.ViewModels;
using Group = HueLib2.Group;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for GroupCreator.xaml
    /// </summary>
    public partial class Form_GroupCreator : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Id of the modified or created group.
        /// </summary>
        private string _id;

        private readonly Bridge _bridge;
        private GroupCreatorViewModel gcvm;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">Actual Bridge.</param>
        public Form_GroupCreator(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            gcvm = this.DataContext as GroupCreatorViewModel;
            List<Light> hr = HueObjectHelper.GetBridgeLights(bridge);
            if(hr != null)
                gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
        }

        public Form_GroupCreator(Group selectedGroup,Bridge bridge)
        {
            InitializeComponent();
            _bridge = bridge;
            gcvm = this.DataContext as GroupCreatorViewModel;
            
            List<Light> hr = HueObjectHelper.GetBridgeLights(bridge);
            if (hr != null)
            {
                gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                Group hr2 = HueObjectHelper.GetObject<Group>(bridge,selectedGroup.Id);
                if (hr2 != null)
                    gcvm.Group = hr2;
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }

            btnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            if (gcvm.Group.Id == null)
            {
                CommandResult<Messages> bresult = _bridge.CreateObject<Group>(gcvm.Group);
                if (bresult.Success)
                {
                    DialogResult = true;
                    log.Info(bresult.Data);
                    _id = bresult.Data.SuccessMessages[0].value;
                    Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);                   
                }
                
            }
            else
            {

                CommandResult<Messages> bresult = _bridge.ModifyObject<Group>(gcvm.Group, gcvm.Group.Id);
                if (bresult.Success)
                {
                    DialogResult = true;
                    _id = gcvm.Group.Id;
                    Close();

                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
