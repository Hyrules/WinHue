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
    /// Interaction logic for Form_RuleConditionCreator.xaml
    /// </summary>
    public partial class Form_RuleConditionCreator : Window
    {
        private RuleConditionViewModel _rcvm;


        public Form_RuleConditionCreator(DataStore ds)
        {
            InitializeComponent();
            _rcvm = DataContext as RuleConditionViewModel;
            _rcvm.Initialize(ds);
        }
/*public RuleCondition RuleCondition
        {
            get { return _rcvm.GetCondition(); }
            set
            {
                
            }
        }
        */
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
