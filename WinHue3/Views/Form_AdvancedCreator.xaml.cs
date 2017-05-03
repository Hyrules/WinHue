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
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for Form_AdvancedCreator.xaml
    /// </summary>
    public partial class Form_AdvancedCreator : Window
    {
        private AdvancedCreatorViewModel _acvm;

        public Form_AdvancedCreator(Bridge bridge)
        {
            InitializeComponent();
            _acvm = DataContext as AdvancedCreatorViewModel;
            _acvm.Initialize(bridge);
        }
    }
}
