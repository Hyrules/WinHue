using System.Collections.Generic;
using System.Windows;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_BulbsView : Window
    {
        private BulbsViewViewModel _bvv;
        private readonly Bridge _bridge;
        public Form_BulbsView(Bridge bridge)
        {
            InitializeComponent();
            _bvv = DataContext as BulbsViewViewModel;
            _bridge = bridge;
            List<Light> lresult = HueObjectHelper.GetObjectsList<Light>(_bridge);
            if (lresult == null) return;
            _bvv.Initialize(lresult);
        }

    }
}
