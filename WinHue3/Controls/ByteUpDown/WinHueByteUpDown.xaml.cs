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
    public partial class WinHueByteUpDown : UserControl
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
            
            if (Value == null)
            {
                Value = byte.MinValue;
            }
            else
            {
                if (Value < byte.MaxValue)
                {
                    Value = Convert.ToByte(Value + Step);
                }
            }

            SetDirtyTextBox();
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            IncrementValue();
        }

        private void DecrementValue()
        {
            if (Value == null)
            {
                Value = byte.MinValue;
            }
            else
            {
                if (Value > byte.MinValue)
                {
                    Value = Convert.ToByte(Value - Step);
                }
            }

            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();
        }

        public byte? Value
        {
            get => (byte?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(byte?), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueByteUpDown nud = d as WinHueByteUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (e.NewValue == null && nud.CanBeNull)
            {
                nud.tbValue.Text = "";
                return;
            }

            nud.tbValue.Text = e.NewValue == null ? byte.MinValue.ToString() : e.NewValue.ToString();
        }

        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeEmptyProperty);
            set => SetValue(CanBeEmptyProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeEmptyProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata(true));

        public byte Step
        {
            get => (byte)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(byte), typeof(WinHueByteUpDown), new FrameworkPropertyMetadata((byte)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValues();
        }

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
                    tbValue.Text = byte.MinValue.ToString();
                }
            }

            if (byte.TryParse(tbValue.Text, out byte val))
            {
                Value = val;
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

            if (!byte.TryParse(tbValue.Text, out byte val))
            {
                btnDecrement.IsEnabled = false;
                btnIncrement.IsEnabled = false;
                return;
            }
            btnDecrement.IsEnabled = Value != byte.MinValue;
            btnIncrement.IsEnabled = Value != byte.MaxValue;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckValues();
        }

        private void TbValue_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ushort.TryParse(tbValue.Text, out ushort i))
                {
                    EnterPressed?.Invoke(this, EventArgs.Empty);
                    ClearDirtyTextBox();
                }

            }
        }

        private void TbValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    SetDirtyTextBox();
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
