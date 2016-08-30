using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_BulbsView : Window
    {
        private BulbsViewView _bvv;
        private Bridge _br;
        public Form_BulbsView(Bridge br)
        {
            InitializeComponent();
            _br = br;
            _bvv = new BulbsViewView(br);
            DataContext = _bvv;
        }
    }
}
