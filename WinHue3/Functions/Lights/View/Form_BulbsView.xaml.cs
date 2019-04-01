using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgeManager;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.Lights.View
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_BulbsView : Window
    {
        private BulbsViewViewModel _bvv;

        public Form_BulbsView()
        {
            InitializeComponent();
            _bvv = DataContext as BulbsViewViewModel;
        }

        public async Task Initialize()
        {
            List<Light> lresult = await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>();
            if (lresult == null) return;
            _bvv.Initialize(lresult);
        }

    }
}
