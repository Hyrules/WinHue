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
    public partial class WinHueTransitionTimeUpDown : UserControl
    {
        public WinHueTransitionTimeUpDown()
        {
            InitializeComponent();
            btnDecrement.IsEnabled = false;
        }

        private static readonly Regex _regex = new Regex(@"^(\d{1,2}|\d\.\d{2}):([0-5]\d):([0-5]\d)(\.\d{3})?$");
        private int carretpos = 0;

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            int finalstep = Step;

            switch (carretpos)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    finalstep = finalstep * 10;
                    break;
                case 3:
                    finalstep = finalstep * 100;
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default:
                    finalstep = 0;
                    break;
            }

            

            Value = Convert.ToUInt16(Value + Step);
            
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
        
            
        }

        public ushort? Value
        {
            get => (ushort?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ushort?), typeof(WinHueTransitionTimeUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueTransitionTimeUpDown tt = d as WinHueTransitionTimeUpDown;
            TimeSpan ts = TimeSpan.FromMilliseconds((ushort)e.NewValue);

            tt.tbValue.Text = ts.ToString("g")
        }

        public ushort Step
        {
            get => (ushort)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(ushort), typeof(WinHueTransitionTimeUpDown), new FrameworkPropertyMetadata((ushort)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {

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

        }

    }
}
