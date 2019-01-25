using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinHue3.Controls.IntegerUpDown
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class WinHueIntegerUpDown : UserControl
    {
        public WinHueIntegerUpDown()
        {
            InitializeComponent();
            btnDecrement.IsEnabled = false;
        }

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private void IncrementValue()
        {
            if (Value == null) Value = Min;
            else if (Value < int.MaxValue && Value < Max)
            {
                Value = Value < Min ? Min : Convert.ToInt32(Value + Step);
            }
                
            SetDirtyTextBox();
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            IncrementValue();            
        }

        private void DecrementValue()
        {
            if (Value == null) Value = Min;
            else if (Value > int.MinValue && Value > Min)
            {
                Value = Value > Max ? Max : Convert.ToInt32(Value - Step);
            }
                
            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();           
        }

        private void SetIncrementalsButtons()
        {
            if (tbValue.Text == "" & CanBeNull)
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

        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeEmptyProperty);
            set => SetValue(CanBeEmptyProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeEmptyProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata(true));

        public int? Value
        {
            get => (int?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int?), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinHueIntegerUpDown nud = d as WinHueIntegerUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (e.NewValue == null && nud.CanBeNull)
            {
                nud.tbValue.Text = "";
                return;
            }
            nud.tbValue.Text = e.NewValue == null ? nud.Min.ToString() : e.NewValue.ToString();

        }

        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata((int)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValues();
            
        }

        private void CheckValues()
        {
            try
            {
                int val = int.Parse(tbValue.Text.TrimStart('0'));

                if (val > Max)
                {
                    Value = Max;
                }

                if (val < Min)
                {
                    Value = Min;
                }

                Value = val;
            }
            catch(OverflowException)
            {
                Value = tbValue.Text[0] == '-' ? Min : Max;
            }
            catch(ArgumentNullException)
            {
                if (CanBeNull)
                    Value = null;
                else
                    tbValue.Text = Min.ToString();
            }
            catch(FormatException){}
            SetIncrementalsButtons();
        }

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(WinHueIntegerUpDown), new PropertyMetadata(int.MaxValue));

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(WinHueIntegerUpDown), new PropertyMetadata(int.MinValue));

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
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) && (e.Key != Key.Subtract) && (e.Key != Key.OemMinus))
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
