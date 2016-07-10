using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HueLib;

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
