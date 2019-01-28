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
            BtnDecrement.IsEnabled = false;
        }

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private static readonly Regex _regex = new Regex(@"^(0[0-1]):([0-5]\d):([0-5]\d)(\.\d{1})?$");
        private int caretpos = 10;

        private void IncrementValue()
        {
            caretpos = TbValue.CaretIndex;
            int finalstep = 1;

            switch (caretpos)
            {
                case 0:
                    break;
                case 1: // 10 hours
                    break;
                case 2: // 1 Hour
                    finalstep = 36000;
                    break;
                case 3: // First : from the left
                    break;
                case 4: // 10 Minutes
                    finalstep = 6000;
                    break;
                case 5: // 1 Minute
                    finalstep = 600;
                    break;
                case 6: // Second : from the left
                    break;
                case 7: // 10 SECONDS
                    finalstep = 100;
                    break;
                case 8: // SECONDS
                    finalstep = 10;
                    break;
                case 9: // Comma
                    break;
                case 10: // 100 of MS
                    finalstep = 1;
                    break;
                default:
                    finalstep = 0;
                    break;
            }

            if (Value == null) Value = ushort.MinValue;
            Value = Convert.ToInt32((Value + finalstep)) > ushort.MaxValue ? ushort.MaxValue : Convert.ToUInt16((Value + finalstep));
            SetIncrementalsButtons();
            SetDirtyTextBox();
            TbValue.CaretIndex = caretpos;
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            IncrementValue();
        }

        private void DecrementValue()
        {
            caretpos = TbValue.CaretIndex;
            int finalstep = 1;

            switch (caretpos)
            {
                case 0:
                    break;
                case 1: // 10 hours
                    break;
                case 2: // 1 Hour
                    finalstep = 36000;
                    break;
                case 3: // First : from the left
                    break;
                case 4: // 10 Minutes
                    finalstep = 6000;
                    break;
                case 5: // 1 Minute
                    finalstep = 600;
                    break;
                case 6: // Second : from the left
                    break;
                case 7: // 10 SECONDS
                    finalstep = 100;
                    break;
                case 8: // SECONDS
                    finalstep = 10;
                    break;
                case 9: // Comma
                    break;
                case 10: // 100 of MS
                    finalstep = 1;
                    break;
                default:
                    finalstep = 0;
                    break;
            }

            if (Value == null) Value = ushort.MinValue;
            Value = Convert.ToInt32((Value - finalstep)) < ushort.MinValue ? ushort.MinValue : Convert.ToUInt16((Value - finalstep));
            SetIncrementalsButtons();
            SetDirtyTextBox();
            TbValue.CaretIndex = caretpos;
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();
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
            DependencyProperty.Register("Value", typeof(ushort?), typeof(WinHueTransitionTimeUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceValue));

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            WinHueTransitionTimeUpDown tt = d as WinHueTransitionTimeUpDown;
            if (basevalue == null)
            {
                tt.TbValue.Text = "";
                tt.BtnDecrement.IsEnabled = false;
                return basevalue;
            }

            int value = (ushort) basevalue * 100;
            TimeSpan ts = TimeSpan.FromMilliseconds(value);

            tt.TbValue.Text = $"{ts:hh\\:mm\\:ss\\.f}";

            return basevalue;
        }

        private void SetIncrementalsButtons()
        {
            if (TbValue.Text == "")
            {
                BtnDecrement.IsEnabled = true;
                BtnIncrement.IsEnabled = true;
                return;
            }

            BtnDecrement.IsEnabled = Value > ushort.MinValue;
            BtnIncrement.IsEnabled = Value < ushort.MaxValue;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            caretpos = TbValue.SelectionStart;
            CheckValues();
        }

        private void TbValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                case Key.Enter:
                case Key.Up:
                case Key.Down:
                    break;
                case Key.Left:
                case Key.Right:
                    caretpos = TbValue.CaretIndex;
                    break;
                default:
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) && e.Key != Key.OemComma && e.Key != Key.OemSemicolon && e.Key != Key.OemPeriod)
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void CheckValues()
        {
            if (TbValue.Text == "")
            {
                Value = null;
            }

            if (_regex.IsMatch(TbValue.Text))
            {
                TimeSpan ts = TimeSpan.Parse(TbValue.Text);
                Value = (ushort)(ts.TotalMilliseconds / 100);
            }

            SetIncrementalsButtons();
        }

        private void TbValue_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            caretpos = TbValue.SelectionStart;
        }


        private void TbValue_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    SetDirtyTextBox();
                    break;
                case Key.Up:
                    IncrementValue();
                    break;
                case Key.Down:
                    DecrementValue();
                    break;
                case Key.Enter:


                    break;
            }
            CheckValues();
        }

        private void SetDirtyTextBox()
        {
            if (EnterPressed != null)
                TbValue.Background = new SolidColorBrush(Color.FromRgb(255, 179, 179));
        }

        private void ClearDirtyTextBox()
        {
            if (EnterPressed != null)
                TbValue.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
        }
    }
}