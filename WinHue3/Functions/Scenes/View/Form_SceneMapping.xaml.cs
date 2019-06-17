using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Scenes.View
{   

    /// <summary>
    /// Interaction logic for Form_SceneMapping.xaml
    /// </summary>
    public partial class Form_SceneMapping : Window
    {
        private Bridge _bridge;
        private readonly SceneMappingViewModel _smv;

        public Form_SceneMapping()
        {
            InitializeComponent();
            _smv = DataContext as SceneMappingViewModel;
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            List<Light> lresult = await _bridge.GetListObjectsAsync<Light>();
            if (lresult != null)
            {
                List<Scene> sresult = await _bridge.GetListObjectsAsync<Scene>();
                if (sresult != null)
                {
                    
                    _smv.Initialize(_bridge,sresult, lresult);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }

        private void dgListScenes_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            if (DgListScenes.Columns.Count < 1) return;
            DgListScenes.Columns[0].Visible = false;
        }

    }
}
