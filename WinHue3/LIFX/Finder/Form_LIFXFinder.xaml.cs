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

namespace WinHue3.LIFX.Finder
{
    /// <summary>
    /// Interaction logic for Form_LIFXFinder.xaml
    /// </summary>
    public partial class Form_LIFXFinder : Window
    {
        LifxFinderViewModel _lfvm;

        public Form_LIFXFinder()
        {
            InitializeComponent();
            _lfvm = this.DataContext as LifxFinderViewModel;    

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
