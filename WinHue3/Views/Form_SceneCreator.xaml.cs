using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class Form_SceneCreator : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Current bridge.
        /// </summary>
        readonly Bridge _bridge;

        /// <summary>
        /// Id of the new or modified scene.
        /// </summary>
        private string _currentsceneid;

        private SceneCreatorView scv;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">active bridge.</param>
        public Form_SceneCreator(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            HelperResult hr = HueObjectHelper.GetObjectsList<Light>(bridge);
            scv = hr.Success ? new SceneCreatorView((List<HueObject>)hr.Hrobject,_bridge) : new SceneCreatorView(new List<HueObject>(),_bridge);
            DataContext = scv;
            _bridge = bridge;
            _currentsceneid = string.Empty;

        }

        public Form_SceneCreator(Bridge bridge, string sceneid)
        {
            InitializeComponent();
            _currentsceneid = sceneid;
            _bridge = bridge;

            HelperResult hr = HueObjectHelper.GetObjectsList<Light>(bridge);
            if (hr.Success)
            {
                scv = new SceneCreatorView((List<HueObject>) hr.Hrobject, sceneid, _bridge);
                DataContext = scv;               
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _currentsceneid;
        }

        private void lbSelectedLights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveLight.IsEnabled = lbSelectedLights.SelectedItem != null;
        }

        private void btnSaveScene_Click(object sender, RoutedEventArgs e)
        {
            Scene newScene = (Scene) scv.GetScene();

            log.Info("Scene to be created : " + newScene);
            CommandResult comres = _currentsceneid == string.Empty? _bridge.CreateObject<Scene>(newScene) : _bridge.ModifyObject<Scene>(newScene,_currentsceneid);

            if (comres.Success)
            {
                MessageCollection mc = ((MessageCollection) comres.resultobject);

                string id = "";
                id = _currentsceneid != string.Empty ? _currentsceneid : ((CreationSuccess)mc[0]).id;
                log.Info("Id of the scene" + id);
                ObservableCollection<HueObject> listLightState = scv.GetSceneLights();
                foreach (HueObject obj in listLightState)
                {
                    _bridge.SetSceneLightState(id, obj.Id, ((Light)obj).state);
                }
                _currentsceneid = id;
                DialogResult = true;
                Close();
            }
            else
            {
                _bridge.ShowErrorMessages();
                log.Error(_bridge.lastMessages);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSelectLight_Click(object sender, RoutedEventArgs e)
        {
            scv.AddLightsToScene(lvAvailableLights.SelectedItems.Cast<HueObject>().ToList());
        }

        private void FormSceneCreator_Loaded(object sender, RoutedEventArgs e)
        {
            if (scv == null)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
