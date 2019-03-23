using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinHue3.Controls.IntegerUpDown
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    [Obsolete]
    public partial class WinHueIntegerUpDown : UserControl
    {
        public WinHueIntegerUpDown()
        {
            InitializeComponent();
            BtnDecrement.IsEnabled = false;
        }

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private void IncrementValue()
        {
            if (string.IsNullOrEmpty(TbValue.Text))
            {
                TbValue.Text = Min.ToString();
            }
            else
            {
                if (int.Parse(TbValue.Text) < Max)
                {
                    TbValue.Text = int.Parse(TbValue.Text) < Min ? Min.ToString() : (int.Parse(TbValue.Text) + Step).ToString();
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
            if (string.IsNullOrEmpty(TbValue.Text))
            {
                TbValue.Text = Min.ToString();
            }
            else
            {
                if (int.Parse(TbValue.Text) > Min)
                {
                    TbValue.Text = int.Parse(TbValue.Text) > Max ? Max.ToString() : (int.Parse(TbValue.Text) - Step).ToString();

                }
            }

            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();           
        }      

        public int? Value
        {
            get => (int?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int?), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceValue));

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            WinHueIntegerUpDown nud = d as WinHueIntegerUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (basevalue == null)
            {
                if (nud.CanBeNull || nud.IsFocused)
                {
                    nud.TbValue.Text = "";
                    return basevalue;
                }
            }
            nud.TbValue.Text = basevalue == null ? nud.Min.ToString() : basevalue.ToString();
            return basevalue;
        }

        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeNullProperty);
            set => SetValue(CanBeNullProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeNullProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata(true));


        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata((int)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private void CheckValues()
        {
            if (TbValue.Text == "")
            {
                if (CanBeNull)
                {
                    Value = null;
                }
                else
                {
                    if (!TbValue.IsFocused)
                        Value = Min;
                }
            }
            else
            {
                if (int.TryParse(TbValue.Text, out int val))
                {
                    Value = val;
                }
                else
                {
                    Value = TbValue.Text.StartsWith("-") ? Min : Max;
                }
            }
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


        private void SetIncrementalsButtons()
        {
            if (TbValue.Text == "" & CanBeNull)
            {
                BtnDecrement.IsEnabled = true;
                BtnIncrement.IsEnabled = true;
                return;
            }

            if (!int.TryParse(TbValue.Text, out int val))
            {
                BtnDecrement.IsEnabled = false;
                BtnIncrement.IsEnabled = false;
                return;
            }
            BtnDecrement.IsEnabled = Value > Min;
            BtnIncrement.IsEnabled = Value < Max;
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
                    if (int.TryParse(TbValue.Text, out int i) && i >= Min && i <= Max)
                    {
                        EnterPressed?.Invoke(this, EventArgs.Empty);
                        ClearDirtyTextBox();
                    }
                    break;
            }

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
                TbValue.Background = new SolidColorBrush(Color.FromRgb(255, 179, 179));
        }

        private void ClearDirtyTextBox()
        {
            if (EnterPressed != null)
                TbValue.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
        }

    }
}
