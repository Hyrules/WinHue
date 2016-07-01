using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HueLib;
using HueLib_base;
using WinHue3.Resources;
using Color = System.Drawing.Color;

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
        string _id = null;

        private SceneCreatorView scv;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">active bridge.</param>
        public Form_SceneCreator(Bridge bridge)
        {
            InitializeComponent();
            scv = new SceneCreatorView(HueObjectHelper.GetBridgeLights(bridge),bridge);
            DataContext = scv;
            _br = bridge;
        }

        public Form_SceneCreator(Bridge bridge, HueObject obj)
        {
            InitializeComponent();
            _id = ((Scene)obj).Id;
            scv = new SceneCreatorView(HueObjectHelper.GetBridgeLights(bridge),obj,bridge);
            DataContext = scv;
            _br = bridge;

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

        private void lbSelectedLights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveLight.IsEnabled = lbSelectedLights.SelectedItem != null;
        }

        private void btnSaveScene_Click(object sender, RoutedEventArgs e)
        {
            Scene newScene = (Scene) scv.GetScene();
            newScene.lastupdated = null;
            newScene.locked = null;
            newScene.recycle = null;
            newScene.version = null;

            log.Info("Scene to be created : " + newScene);
            _id = _id == "" ? _br.CreateScene(newScene) : _br.ChangeScene(_id, newScene.name,newScene.lights);
            log.Info("Id of the new scene" + _id);

            if (!string.IsNullOrEmpty(_id))
            {
                ObservableCollection<HueObject> listLightState = scv.GetSceneLights();
                foreach (HueObject obj in listLightState)
                {
                    _br.SetSceneLightState(_id, obj.Id, ((Light) obj).state);
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
