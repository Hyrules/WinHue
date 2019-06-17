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
        private Bridge _bridge;

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

        public async Task Initialize(Bridge bridge,Group selectedGroup = null)
        {
            _bridge = bridge;
            List<Light> hr = await _bridge.GetListObjectsAsync<Light>();
            if (selectedGroup == null)
            {            
                if (hr != null)
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);
            }
            else
            {
                if (hr != null)
                {
                    _id = selectedGroup.Id;
                    gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(hr);

                    Group hr2 = await _bridge.GetObjectAsync<Group>(selectedGroup.Id);
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
                BtnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
            }
        }

        public async Task Initialize(string group)
        {
            List<Light> hr = await _bridge.GetListObjectsAsync<Light>();
            _id = group;
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

                    Group hr2 = await _bridge.GetObjectAsync<Group>(group);
                    if (hr2 != null)
                        gcvm.Group = hr2;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
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
            bool result = false;
            if (_id == null)
            {
                result = _bridge.CreateObject(gcvm.Group);
                if (result)
                {
                    log.Info("Group creation success");
                    _id = _bridge.LastCommandMessages.LastSuccess.value;
                }
            }
            else
            {
                result = _bridge.ModifyObject(gcvm.Group);
                if (result)
                {
                    log.Info("Group modification success");
                }
            }

            if (!result)
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }

            DialogResult = _id != null;
            if (DialogResult.GetValueOrDefault()) Close();
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
