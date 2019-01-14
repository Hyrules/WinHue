using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.Controls.UShortUpDown
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class WinHueUShortUpDown : UserControl
    {
        public WinHueUShortUpDown()
        {
            InitializeComponent();
            btnDecrement.IsEnabled = false;
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (Value == null) Value = ushort.MinValue;
            else
            if (Value < ushort.MaxValue)
                Value = Convert.ToUInt16(Value + Step);
            
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            if (Value == null) Value = ushort.MinValue;
            else
            if (Value > ushort.MinValue)
                Value = Convert.ToUInt16(Value - Step);
            
        }

        public ushort? Value
        {
            get => (ushort?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ushort?), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueUShortUpDown nud = d as WinHueUShortUpDown;
            if (e.NewValue == null)
            {
                nud.tbValue.Text = "";
                return;
            }
            nud.tbValue.Text = e.NewValue.ToString();
            if (nud.btnIncrement == null) return;
            nud.btnIncrement.IsEnabled = ushort.Parse(e.NewValue.ToString()) != ushort.MaxValue;
            nud.btnDecrement.IsEnabled = ushort.Parse(e.NewValue.ToString()) != ushort.MinValue;
        }

        private ushort Step
        {
            get { return (ushort)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(ushort), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata((ushort)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbValue.Text == "")
            {
                Value = null;
                return;
            }
            if (!ushort.TryParse(tbValue.Text, out ushort val))
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
            ushort val = 0;
            if (tbValue.Text == "") return;
            while (!ushort.TryParse(tbValue.Text, out val))
            {
                tbValue.Text = tbValue.Text.Remove(tbValue.Text.Length - 1);
            }

            Value = val;
            btnDecrement.IsEnabled = true;
            btnIncrement.IsEnabled = true;
        }
    }
}
