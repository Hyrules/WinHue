using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
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
        private Bridge _bridge;

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

        public async Task Inititalize(Bridge bridge,string sceneid = null)
        {
            _bridge = bridge;
            _currentsceneid = sceneid ?? string.Empty;

            List<Light> hr = await _bridge.GetListObjectsAsync<Light>();

            if (hr != null)
            {
                Scene cr = sceneid != null ? _bridge.GetObject<Scene>(sceneid) : null;
                _currentsceneid = sceneid;
                _scvm.Initialize(bridge,hr, cr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
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
                result = _bridge.CreateObject(newScene);
            }
            else
            {
                newScene.Id = _currentsceneid;
                result = _bridge.ModifyObject(newScene);
            }
            
            if (result)
            {
                string id = _currentsceneid ?? _bridge.LastCommandMessages.LastSuccess.value;
                log.Info("Id of the scene" + id);
                _currentsceneid = id;
                DialogResult = true;
                Close();
            }
            else
            {
                _bridge.ShowErrorMessages();
                log.Error(_bridge.LastCommandMessages);
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }




    }
}
