using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_GroupView : Window
    {
        private GroupViewView _gvv;
        private Bridge _br;
        public Form_GroupView(Bridge br)
        {
            InitializeComponent();
            _br = br;
            _gvv = new GroupViewView(br);
            DataContext = _gvv;
        }
    }
}
