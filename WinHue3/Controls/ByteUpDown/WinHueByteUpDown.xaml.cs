using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WinHue3.Controls.ByteUpDown
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    [Obsolete]
    public partial class WinHueByteUpDown
    {
        public WinHueByteUpDown()
        {
            InitializeComponent();
            btnDecrement.IsEnabled = false;
        }

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private void IncrementValue()
        {
            
            if (string.IsNullOrEmpty(tbValue.Text))
            {
                tbValue.Text = byte.MinValue.ToString();
            }
            else
            {
                if (byte.Parse(tbValue.Text) < byte.MaxValue)
                {
                    tbValue.Text = (byte.Parse(tbValue.Text) + Step).ToString();
                }
            }

            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            IncrementValue();
        }

        private void DecrementValue()
        {
            if (string.IsNullOrEmpty(tbValue.Text))
            {
                tbValue.Text = byte.MinValue.ToString();
            }
            else
            {
                if(byte.Parse(tbValue.Text) > byte.MinValue)
                {
                    tbValue.Text = (byte.Parse(tbValue.Text) - Step).ToString();
                }
            }

            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();
        }

        public byte? Value
        {
            get => (byte?) GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                ValueChanged?.Invoke(this,null);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(byte?), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceValue));

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            WinHueByteUpDown nud = d as WinHueByteUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (basevalue == null)
            {
                if (nud.CanBeNull || nud.IsFocused)
                {
                    nud.tbValue.Text = "";
                    return null;
                }

            }
            nud.tbValue.Text = basevalue == null ? byte.MinValue.ToString() : basevalue.ToString();
            nud.SetIncrementalsButtons();
            return basevalue;
        }

        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeNullProperty);
            set => SetValue(CanBeNullProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeNullProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata(true));

        public byte Step
        {
            get => (byte)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(byte), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata((byte)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void CheckValues()
        {
            if (tbValue.Text == "")
            {
                if (CanBeNull)
                {
                    Value = null;
                }
                else
                {
                    if (!tbValue.IsFocused)
                        Value = byte.MinValue;
                }
            }
            else
            {
                Value = byte.TryParse(tbValue.Text, out byte val) ? val : byte.MaxValue;
            }

            SetIncrementalsButtons();
        }

        private void SetIncrementalsButtons()
        {
            if(tbValue.Text == "" & CanBeNull)
            {
                btnDecrement.IsEnabled = true;
                btnIncrement.IsEnabled = true;
                return;
            }

            if (!byte.TryParse(tbValue.Text, out _))
            {
                btnDecrement.IsEnabled = false;
                btnIncrement.IsEnabled = false;
                return;
            }
            btnDecrement.IsEnabled = Value > byte.MinValue;
            btnIncrement.IsEnabled = Value < byte.MaxValue;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckValues();
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
                    if (byte.TryParse(tbValue.Text, out _))
                    {
                        EnterPressed?.Invoke(this, EventArgs.Empty);
                        ClearDirtyTextBox();
                        return;
                    }
                    break;
            }
            CheckValues();
        }

        private void TbValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete: // IGNORE THOSE KEYSTROKES WE WANT THEM
                case Key.Back:
                case Key.Enter:
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    break;
                default:
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                    {
                        e.Handled = true;
                    }
                    break;
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
