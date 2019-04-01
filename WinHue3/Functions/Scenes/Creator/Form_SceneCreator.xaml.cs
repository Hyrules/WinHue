using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgeManager;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Scenes.Creator
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class Form_SceneCreator : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Id of the new or modified scene.
        /// </summary>
        private string _currentsceneid;

        private SceneCreatorViewModel _scvm;

        /// <summary>
        /// ctor
        /// </summary>
        public Form_SceneCreator()
        {
            InitializeComponent();
            _scvm = DataContext as SceneCreatorViewModel;

        }

        public async Task Inititalize(string sceneid = null)
        {

            _currentsceneid = sceneid ?? string.Empty;

            List<Light> hr = await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>();

            if (hr != null)
            {
                Scene cr = sceneid != null ? BridgesManager.Instance.SelectedBridge.GetObject<Scene>(sceneid) : null;
                _currentsceneid = sceneid;
                _scvm.Initialize(hr, cr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgesManager.Instance.SelectedBridge);
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _currentsceneid;
        }

        private void btnSaveScene_Click(object sender, RoutedEventArgs e)
        {
            Scene newScene = _scvm.Scene;
            bool result;

            log.Info("Scene to be created : " + newScene);

            if (_currentsceneid == null)
            {
                result = BridgesManager.Instance.SelectedBridge.CreateObject(newScene);
            }
            else
            {
                newScene.Id = _currentsceneid;
                result = BridgesManager.Instance.SelectedBridge.ModifyObject(newScene);
            }
            
            if (result)
            {
                string id = _currentsceneid ?? BridgesManager.Instance.SelectedBridge.LastCommandMessages.LastSuccess.value;
                log.Info("Id of the scene" + id);
                _currentsceneid = id;
                DialogResult = true;
                Close();
            }
            else
            {
                BridgesManager.Instance.SelectedBridge.ShowErrorMessages();
                log.Error(BridgesManager.Instance.SelectedBridge.LastCommandMessages);
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }




    }
}
