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
        readonly Bridge _br;

        /// <summary>
        /// Id of the new or modified scene.
        /// </summary>
        private Scene _currentscene;

        private SceneCreatorView scv;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">active bridge.</param>
        public Form_SceneCreator(Bridge bridge)
        {
            InitializeComponent();
            HelperResult hr = HueObjectHelper.GetObjectsList<Light>(bridge);
            scv = hr.Success ? new SceneCreatorView((List<HueObject>)hr.Hrobject, bridge) : new SceneCreatorView(new List<HueObject>(), bridge);
            DataContext = scv;
            _br = bridge;
        }

        public Form_SceneCreator(Bridge bridge, HueObject obj)
        {
            InitializeComponent();
            _currentscene = ((Scene)obj);
            HelperResult hr = HueObjectHelper.GetObjectsList<Light>(bridge);
            if (hr.Success)
            {
                scv = new SceneCreatorView((List<HueObject>) hr.Hrobject, obj, bridge);
            }
            else
            {
                scv = new SceneCreatorView(new List<HueObject>(), bridge);
            }
            DataContext = scv;
            _br = bridge;
        }

        public string GetCreatedOrModifiedID()
        {
            return _currentscene.Id;
        }

        private void lbSelectedLights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveLight.IsEnabled = lbSelectedLights.SelectedItem != null;
        }

        private void btnSaveScene_Click(object sender, RoutedEventArgs e)
        {
            Scene newScene = (Scene) scv.GetScene();

            log.Info("Scene to be created : " + newScene);
            CommandResult comres = _currentscene == null? _br.CreateObject<Scene>(newScene) : _br.ModifyObject<Scene>(newScene,_currentscene.Id);

            if (comres.Success)
            {
                MessageCollection mc = ((MessageCollection) comres.resultobject);
                string id = ((Success) mc[0]).id;                    
                log.Info("Id of the scene" + id);
                ObservableCollection<HueObject> listLightState = scv.GetSceneLights();
                foreach (HueObject obj in listLightState)
                {
                    _br.SetSceneLightState(id, obj.Id, ((Light)obj).state);
                }

                DialogResult = true;
                Close();
            }
            else
            {
                _br.ShowErrorMessages();
                log.Error(_br.lastMessages);
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

   

    }
}
