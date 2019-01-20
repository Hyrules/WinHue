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

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private static readonly Regex _regex = new Regex(@"^(0[0-1]):([0-5]\d):([0-5]\d)(\.\d{1})?$");
        private int caretpos = 10;

        private void IncrementValue()
        {
            caretpos = tbValue.CaretIndex;
            int finalstep = Step;

            switch (caretpos)
            {
                case 0:
                    break;
                case 1: // 10 hours
                    break;
                case 2: // 1 Hour
                    finalstep = finalstep * 36000;
                    break;
                case 3: // First : from the left
                    break;
                case 4: // 10 Minutes
                    finalstep = finalstep * 6000;
                    break;
                case 5: // 1 Minute
                    finalstep = finalstep * 600;
                    break;
                case 6: // Second : from the left
                    break;
                case 7: // 10 SECONDS
                    finalstep = finalstep * 100;
                    break;
                case 8: // SECONDS
                    finalstep = finalstep * 10;
                    break;
                case 9: // Comma
                    break;
                case 10: // 100 of MS
                    finalstep = finalstep * 1;
                    break;
                default:
                    finalstep = 0;
                    break;
            }

            if (Value == null) Value = ushort.MinValue;

            Value = Convert.ToInt32((Value + finalstep)) > ushort.MaxValue ? ushort.MaxValue : Convert.ToUInt16((Value + finalstep));
            btnDecrement.IsEnabled = Value > ushort.MinValue;
            btnIncrement.IsEnabled = Value < ushort.MaxValue;
            tbValue.CaretIndex = caretpos;
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {          
            IncrementValue();
        }

        private void DecrementValue()
        {
            caretpos = tbValue.CaretIndex;
            int finalstep = Step;

            switch (caretpos)
            {
                case 0:
                    break;
                case 1: // 10 hours
                    break;
                case 2: // 1 Hour
                    finalstep = finalstep * 36000;
                    break;
                case 3: // First : from the left
                    break;
                case 4: // 10 Minutes
                    finalstep = finalstep * 6000;
                    break;
                case 5: // 1 Minute
                    finalstep = finalstep * 600;
                    break;
                case 6: // Second : from the left
                    break;
                case 7: // 10 SECONDS
                    finalstep = finalstep * 100;
                    break;
                case 8: // SECONDS
                    finalstep = finalstep * 10;
                    break;
                case 9: // Comma
                    break;
                case 10: // 100 of MS
                    finalstep = finalstep * 1;
                    break;
                default:
                    finalstep = 0;
                    break;
            }

            if (Value == null) Value = ushort.MinValue;
            Value = Convert.ToInt32((Value - finalstep)) < ushort.MinValue ? ushort.MinValue : Convert.ToUInt16((Value - finalstep));
            btnDecrement.IsEnabled = Value > ushort.MinValue;
            btnIncrement.IsEnabled = Value < ushort.MaxValue;
            SetDirtyTextBox();
            tbValue.CaretIndex = caretpos;

        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            caretpos = tbValue.CaretIndex;
            DecrementValue();
            SetDirtyTextBox();
            tbValue.CaretIndex = caretpos;

        }

        public ushort? Value
        {
            get => (ushort?) GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ushort?), typeof(WinHueTransitionTimeUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueTransitionTimeUpDown tt = d as WinHueTransitionTimeUpDown;
            if (e.NewValue == null)
            {
                tt.tbValue.Text = "";
                tt.btnDecrement.IsEnabled = false;
                return;
            }

            int value =  (ushort)e.NewValue * 100;
            TimeSpan ts = TimeSpan.FromMilliseconds(value);

            tt.tbValue.Text = $"{ts:hh\\:mm\\:ss\\.f}";
            tt.btnDecrement.IsEnabled = value > ushort.MinValue;
            tt.btnIncrement.IsEnabled = value < ushort.MaxValue * 100;

        }

        public ushort Step
        {
            get => (ushort) GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(ushort), typeof(WinHueTransitionTimeUpDown), new FrameworkPropertyMetadata((ushort) 1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            caretpos = tbValue.SelectionStart;
        }

        private void TbValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.Key)
            {
                case Key.Delete:
                    if (tbValue.Text[tbValue.CaretIndex] == ':' || tbValue.Text[tbValue.CaretIndex] == '.')
                        e.Handled = true;
                    break;
                case Key.Back:
                    if (tbValue.Text[tbValue.CaretIndex -1] == ':' || tbValue.Text[tbValue.CaretIndex - 1] == '.')
                        e.Handled = true;
                    break;
                case Key.Enter:
                    break;
                case Key.Up:                  
                    IncrementValue();
                    break;
                case Key.Down:
                    DecrementValue();
                    break;
                case Key.Left:
                case Key.Right:
                    caretpos = tbValue.CaretIndex;
                    break;
                default:
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void TbValue_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            caretpos = tbValue.SelectionStart;
        }

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_regex.IsMatch(tbValue.Text))
            {
                TimeSpan ts = TimeSpan.Parse(tbValue.Text);
                Value = (ushort)(ts.TotalMilliseconds / 100);
            }
        }

        private void TbValue_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_regex.IsMatch(tbValue.Text))
                {
                    EnterPressed?.Invoke(this, EventArgs.Empty);
                    ClearDirtyTextBox();
                }
            }
        }

        private void SetDirtyTextBox()
        {
            if (EnterPressed != null)
                tbValue.Background = new SolidColorBrush(Color.FromRgb(255, 179, 179));
        }

        private void ClearDirtyTextBox()
        {
            if (EnterPressed != null)
                tbValue.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
        }
    }
}