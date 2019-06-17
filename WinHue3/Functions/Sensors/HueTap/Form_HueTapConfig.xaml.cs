using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;

namespace WinHue3.Functions.Sensors.HueTap
{
    /// <summary>
    /// Interaction logic for Form_HueTapConfig.xaml
    /// </summary>
    public partial class Form_HueTapConfig : Window
    {
        private HueTapConfigViewModel tcvm;
        private Bridge _bridge;
        public Form_HueTapConfig()
        {

            InitializeComponent();
            tcvm = DataContext as HueTapConfigViewModel;  
        }

        public async Task Initialize(Bridge bridge,string sensorid)
        {
            _bridge = bridge;
            tcvm.Initialize(_bridge);
            tcvm.HueTapModel.Id = sensorid;

            List<Scene> hr = await _bridge.GetListObjectsAsync<Scene>();

            if (hr != null)
            {

                if (WinHueSettings.settings.ShowHiddenScenes)
                {
                    tcvm.HueTapModel.ListScenes = hr;
                }
                else
                {
                    List<Scene> temp = hr;
                    temp = temp.Where(x => !x.name.StartsWith("HIDDEN")).ToList();
                    tcvm.HueTapModel.ListScenes = temp;
                }

            }
            else
            {
                _bridge.ShowErrorMessages();
            }
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
