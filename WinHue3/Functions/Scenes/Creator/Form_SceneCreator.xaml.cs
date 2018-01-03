using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;
using Bridge = WinHue3.Philips_Hue.BridgeObject.Bridge;

namespace WinHue3.Functions.Scenes.Creator
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class Form_SceneCreator : MetroWindow
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Current bridge.
        /// </summary>
        private Bridge _bridge;

        /// <summary>
        /// Id of the new or modified scene.
        /// </summary>
        private string _currentsceneid;

        private SceneCreatorViewModel _scvm;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">active bridge.</param>
        public Form_SceneCreator()
        {
            InitializeComponent();
            _scvm = DataContext as SceneCreatorViewModel;

        }

        public async Task Inititalize(Bridge bridge, string sceneid = null)
        {
            _bridge = bridge;
            _currentsceneid = sceneid ?? string.Empty;

            List<Light> hr = await HueObjectHelper.GetBridgeLightsAsyncTask(bridge);
            if (hr != null)
            {
                _scvm.Initialize(hr, _bridge);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }

            if (sceneid != null)
            {
                _currentsceneid = sceneid;
                Scene cr = _bridge.GetObject<Scene>(sceneid);
                if (cr != null)
                {
                    _scvm.Initialize(_bridge);
                    _scvm.Scene = cr;
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
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

            if (_currentsceneid == string.Empty)
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
                string id = _currentsceneid != string.Empty ? _currentsceneid : _bridge.LastCommandMessages.LastSuccess.value;
                log.Info("Id of the scene" + id);
                
                foreach (KeyValuePair<string,State> obj in newScene.lightstates)
                {
                    _bridge.SetSceneLightState(id, obj.Key, obj.Value);
                }
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
