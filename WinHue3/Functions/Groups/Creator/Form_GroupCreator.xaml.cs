using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Resources;
using WinHue3.Utils;

namespace WinHue3.Functions.Groups.Creator
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

        private GroupCreatorViewModel gcvm;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">Actual Bridge.</param>
        public Form_GroupCreator()
        {
            InitializeComponent();
            gcvm = this.DataContext as GroupCreatorViewModel;
        }

        public async Task Initialize(Group selectedGroup = null)
        {

            if (selectedGroup == null)
            {
                List<Light> hr = await BridgeManager.SelectedBridge.GetBridgeLightsAsyncTask();
                if (hr != null)
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
            }
            else
            {
                List<Light> hr = await BridgeManager.SelectedBridge.GetBridgeLightsAsyncTask();
                if (hr != null)
                {
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                    Group hr2 = (Group)await BridgeManager.SelectedBridge.GetObjectAsyncTask(BridgeManager.SelectedBridge, selectedGroup.Id, typeof(Group));
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.SelectedBridge);
                }
                BtnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
            }
        }

        public async Task Initialize(string group)
        {
            if (string.IsNullOrEmpty(group))
            {
                List<Light> hr = await BridgeManager.SelectedBridge.GetBridgeLightsAsyncTask();
                if (hr != null)
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
            }
            else
            {
                List<Light> hr = await BridgeManager.SelectedBridge.GetBridgeLightsAsyncTask();
                if (hr != null)
                {
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                    Group hr2 = (Group)await BridgeManager.SelectedBridge.GetObjectAsyncTask(BridgeManager.SelectedBridge, group, typeof(Group));
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.SelectedBridge);
                }
                BtnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
            }
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
                bool result = BridgeManager.SelectedBridge.CreateObject(gcvm.Group);
                if (result)
                {
                    DialogResult = true;
                    log.Info("Group creation success");
                    _id = BridgeManager.SelectedBridge.LastCommandMessages.LastSuccess.value;
                    Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.SelectedBridge);                   
                }
                
            }
            else
            {
                bool result = BridgeManager.SelectedBridge.ModifyObject(gcvm.Group);
                if (result)
                {
                    DialogResult = true;
                    _id = gcvm.Group.Id;
                    Close();

                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.SelectedBridge);
                }
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
