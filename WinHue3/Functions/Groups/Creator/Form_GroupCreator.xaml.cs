using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgeManager;
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
            gcvm = DataContext as GroupCreatorViewModel;
        }

        public async Task Initialize(Group selectedGroup = null)
        {
            List<Light> hr = await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>();
            if (selectedGroup == null)
            {            
                if (hr != null)
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
            }
            else
            {
                if (hr != null)
                {
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                    Group hr2 = await BridgesManager.Instance.SelectedBridge.GetObjectAsync<Group>(selectedGroup.Id);
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgesManager.Instance.SelectedBridge);
                }
                BtnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
            }
        }

        public async Task Initialize(string group)
        {
            List<Light> hr = await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>();
            if (string.IsNullOrEmpty(group))
            {
                
                if (hr != null)
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
            }
            else
            {
                if (hr != null)
                {
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                    Group hr2 = await BridgesManager.Instance.SelectedBridge.GetObjectAsync<Group>(group);
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgesManager.Instance.SelectedBridge);
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
            _id = gcvm.CreateGroup();
            DialogResult = _id != null;
            if (DialogResult.GetValueOrDefault()) Close();
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
