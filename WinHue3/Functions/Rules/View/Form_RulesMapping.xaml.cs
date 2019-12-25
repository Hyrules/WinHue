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

namespace WinHue3.Functions.Rules.View
{
    /// <summary>
    /// Interaction logic for Form_RulesViewMapping.xaml
    /// </summary>
    public partial class Form_RulesMapping : Window
    {
        private RulesMappingViewModel _rmvm;
        public Form_RulesMapping()
        {
            InitializeComponent();
            _rmvm = new RulesMappingViewModel();
        }
    }
}
