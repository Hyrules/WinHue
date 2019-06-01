using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Groups.View
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_GroupView : Window
    {
        private GroupViewViewModel _gvv;
        private Bridge _bridge;
        public Form_GroupView()
        {
            InitializeComponent();
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            List<Light> comlgt = await _bridge.GetListObjectsAsync<Light>();

            _gvv = DataContext as GroupViewViewModel;

            if (comlgt != null)
            {
                List<Group> comgrp = await _bridge.GetListObjectsAsync<Group>();
                if (comgrp != null)
                {

                    _gvv.Initialize(comgrp, comlgt);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }

                DataContext = _gvv;

            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }

    }
}
