using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private void IncrementValue()
        {
            if (string.IsNullOrEmpty(tbValue.Text))
            {
                tbValue.Text = Min.ToString();
            }
            else
            {
                if (ushort.Parse(tbValue.Text) < Max)
                {
                    tbValue.Text = ushort.Parse(tbValue.Text) < Min ? Min.ToString() : (ushort.Parse(tbValue.Text) + Step).ToString();
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
                tbValue.Text = Min.ToString();
            }
            else
            {
                if (ushort.Parse(tbValue.Text) > Min)
                {
                    tbValue.Text = ushort.Parse(tbValue.Text) > Max ? Max.ToString() : (ushort.Parse(tbValue.Text) - Step).ToString();
                }
            }

            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();           
        }

        public ushort? Value
        {
            get => (ushort?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ushort?), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null , CoerceValue));

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            WinHueUShortUpDown nud = d as WinHueUShortUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (basevalue == null)
            {
                if (nud.CanBeNull || nud.IsFocused)
                {
                    nud.tbValue.Text = "";
                    return basevalue;
                }
            }
            nud.tbValue.Text = basevalue == null ? nud.Min.ToString() : basevalue.ToString();
            nud.SetIncrementalsButtons();
            return basevalue;
        }

        private ushort Step
        {
            get { return (ushort)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(ushort), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata((ushort)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeNullProperty);
            set => SetValue(CanBeNullProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeNullProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata(true));

        public ushort Min
        {
            get { return (ushort)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(ushort), typeof(WinHueUShortUpDown), new FrameworkPropertyMetadata(ushort.MinValue));


        public ushort Max
        {
            get { return (ushort)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(ushort), typeof(WinHueUShortUpDown), new PropertyMetadata(ushort.MaxValue));


        private void SetIncrementalsButtons()
        {
            if (tbValue.Text == "" & CanBeNull)
            {
                btnDecrement.IsEnabled = true;
                btnIncrement.IsEnabled = true;
                return;
            }

            if (!ushort.TryParse(tbValue.Text, out ushort val))
            {
                btnDecrement.IsEnabled = false;
                btnIncrement.IsEnabled = false;
                return;
            }
            btnDecrement.IsEnabled = Value > Min;
            btnIncrement.IsEnabled = Value < Max;
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
                    if (!tbValue.IsFocused)
                        Value = Min;
                }
            }
            else
            {
                Value = ushort.TryParse(tbValue.Text, out ushort val) ? val : Max;
            }

            SetIncrementalsButtons();
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
                    if (ushort.TryParse(tbValue.Text, out ushort i) && i >= Min && i <= Max)
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
                case Key.Delete:
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
