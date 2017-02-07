using System;
using System.Collections.Generic;
using System.Windows;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{   

    /// <summary>
    /// Interaction logic for Form_SceneMapping.xaml
    /// </summary>
    public partial class Form_SceneMapping : Window
    {

        private readonly SceneMappingViewModel _smv;
        private readonly Bridge _bridge;
        public Form_SceneMapping(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            CommandResult lresult = _bridge.GetListObjects<Light>();
            if (lresult.Success)
            {
                CommandResult sresult = _bridge.GetListObjects<Scene>();
                if (sresult.Success)
                {
                    _smv = DataContext as SceneMappingViewModel;
                    _smv.Initialize((Dictionary<string, Scene>)sresult.resultobject, (Dictionary<string, Light>)lresult.resultobject, _bridge);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(bridge);
                }
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(bridge);
            }
            

        }

        private void dgListScenes_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            if (dgListScenes.Columns.Count < 1) return;
            dgListScenes.Columns[0].Visible = false;
        }

    }
}
