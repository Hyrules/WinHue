using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.Lights.View
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_BulbsView : Window
    {
        private BulbsViewViewModel _bvv;
        private Bridge _bridge;
        public Form_BulbsView()
        {
            InitializeComponent();
            _bvv = DataContext as BulbsViewViewModel;
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            List<Light> lresult = await _bridge.GetListObjectsAsync<Light>();
            if (lresult == null) return;
            _bvv.Initialize(lresult);
        }

    }
}
