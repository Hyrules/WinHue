using System;
using System.Collections.Generic;
using System.Windows;
using HueLib2;

namespace WinHue3
{   

    /// <summary>
    /// Interaction logic for Form_SceneMapping.xaml
    /// </summary>
    public partial class Form_SceneMapping : Window
    {
        private Bridge _bridge;
        private readonly SceneMappingView _smv;
        public Form_SceneMapping()
        {
            InitializeComponent();
            CommandResult lresult = BridgeStore.SelectedBridge.GetListObjects<Light>();
            if (!lresult.Success) return;
            CommandResult sresult = BridgeStore.SelectedBridge.GetListObjects<Scene>();
            if (!sresult.Success) return;
            _smv = new SceneMappingView((Dictionary<string, Scene>)sresult.resultobject,
                (Dictionary<string, Light>)lresult.resultobject);
            DataContext = _smv;
        }

        private void dgListScenes_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            if (dgListScenes.Columns.Count < 1) return;
            dgListScenes.Columns[0].Visible = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(_smv == null)
                Close();
        }
    }
}
