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
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Animations
{
    /// <summary>
    /// Logique d'interaction pour Form_Animations.xaml
    /// </summary>
    public partial class Form_Animations : Window
    {
        private AnimationCreatorViewModel _acvm;

        public Form_Animations()
        {
            InitializeComponent();
            _acvm = this.DataContext as AnimationCreatorViewModel;
        }


    }
}
