using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WinHue3.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class WinHueXYUpDown : UserControl
    {
        public WinHueXYUpDown()
        {
            InitializeComponent();
            btnDecrement.IsEnabled = false;
        }

        private static readonly Regex _regex = new Regex("[^0-9]+.");

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (Value == null) Value = (decimal)0.000;
            else
            if (Value < 1)
                Value = Convert.ToDecimal(Value + Step);
            
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            if (Value == null) Value = (decimal)0.000;
            else
            if (Value > 0)
                Value = Convert.ToDecimal(Value - Step);
            
        }

        public decimal? Value
        {
            get => (decimal?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(WinHueXYUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueXYUpDown nud = d as WinHueXYUpDown;
            if (e.NewValue == null)
            {
                nud.tbValue.Text = "";
                return;
            }
            nud.tbValue.Text = e.NewValue.ToString();
            if (nud.btnIncrement == null) return;
            nud.btnIncrement.IsEnabled = decimal.Parse(e.NewValue.ToString()) < 1;
            nud.btnDecrement.IsEnabled = decimal.Parse(e.NewValue.ToString()) > 0;
        }

        public decimal Step
        {
            get => (decimal)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(decimal), typeof(WinHueXYUpDown), new FrameworkPropertyMetadata((decimal)0.001,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbValue.Text == "")
            {
                Value = null;
                return;
            }
            if (!decimal.TryParse(tbValue.Text, out decimal val))
            {               
                btnDecrement.IsEnabled = false;
                btnIncrement.IsEnabled = false;
            }
            else
            {
                Value = val;
            }
        }

        private void TbValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal val = (decimal)0.000;
            if (tbValue.Text == "") return;
            while (!decimal.TryParse(tbValue.Text, out val))
            {
                tbValue.Text = tbValue.Text.Remove(tbValue.Text.Length - 1);
            }

            Value = val;
            btnDecrement.IsEnabled = true;
            btnIncrement.IsEnabled = true;
        }
    }
}
