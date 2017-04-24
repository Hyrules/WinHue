using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_HueTapConfig.xaml
    /// </summary>
    public partial class Form_HueTapConfig : Window
    {
        private readonly Bridge _bridge;

        public Form_HueTapConfig(string sensorid,Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            HueTapConfigViewModel tcvm = this.DataContext as HueTapConfigViewModel;
            tcvm.Bridge = bridge;
            tcvm.HueTapModel.Id = sensorid;


            HelperResult hr = HueObjectHelper.GetBridgeScenes(_bridge);            

            if (hr.Success)
            {

                if (WinHueSettings.settings.ShowHiddenScenes)
                {
                    tcvm.HueTapModel.ListScenes = (List<Scene>)hr.Hrobject;
                }
                else
                {
                    List<Scene> temp = ((List<Scene>)hr.Hrobject);
                    temp = temp.Where(x => !x.GetName().StartsWith("HIDDEN")).ToList();
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
