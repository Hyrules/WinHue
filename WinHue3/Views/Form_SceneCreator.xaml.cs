using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HueLib2;
using WinHue3.ViewModels;

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

        private SceneCreatorViewModel _scvm;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">active bridge.</param>
        public Form_SceneCreator(Bridge bridge, string sceneid = null)
        {
            InitializeComponent();
            _bridge = bridge;
            _scvm = DataContext as SceneCreatorViewModel;
            HelperResult hr = HueObjectHelper.GetObjectsList<Light>(bridge);
            if (hr.Success)
            {
                _scvm.Initialize((List<Light>)hr.Hrobject,_bridge);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }

            if (sceneid != null)
            {
                _currentsceneid = sceneid;
                CommandResult cr = _bridge.GetObject<Scene>(sceneid);
                if (cr.Success)
                {
                    _scvm.Initialize(_bridge);
                    _scvm.Scene = (Scene) cr.resultobject;
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

            log.Info("Scene to be created : " + newScene);
            CommandResult comres = _currentsceneid == string.Empty? _bridge.CreateObject<Scene>((Scene)newScene.Clone()) : _bridge.ModifyObject<Scene>((Scene)newScene.Clone(),_currentsceneid);

            if (comres.Success)
            {
                MessageCollection mc = ((MessageCollection) comres.resultobject);

                string id = "";
                id = _currentsceneid != string.Empty ? _currentsceneid : ((CreationSuccess)mc[0]).id;
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
                log.Error(_bridge.lastMessages);
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }




    }
}
