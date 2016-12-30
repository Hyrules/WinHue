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

        public Form_HueTapConfig(string sensorid)
        {
            InitializeComponent();
            HueTapConfigViewModel tcvm = this.DataContext as HueTapConfigViewModel;
            tcvm.HueTapModel.Id = sensorid;


            HelperResult hr = HueObjectHelper.GetBridgeScenes(BridgeStore.SelectedBridge);            

            if (hr.Success)
            {

                if (WinHueSettings.settings.ShowHiddenScenes)
                {
                    tcvm.HueTapModel.ListScenes = (List<HueObject>)hr.Hrobject;
                }
                else
                {
                    List<HueObject> temp = ((List<HueObject>)hr.Hrobject);
                    temp = temp.Where(x => !x.GetName().StartsWith("HIDDEN")).ToList();
                    tcvm.HueTapModel.ListScenes = temp;
                }

            }
            else
            {
                BridgeStore.SelectedBridge.ShowErrorMessages();
            }
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
