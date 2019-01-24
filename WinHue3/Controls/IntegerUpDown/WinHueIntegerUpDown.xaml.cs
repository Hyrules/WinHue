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
            if (e.NewValue == null && nud.CanBeEmpty)
            {
                nud.tbValue.Text = "";
                return;
            }
            nud.tbValue.Text = e.NewValue.ToString();
            if (nud.btnIncrement == null) return;
            nud.btnIncrement.IsEnabled = int.Parse(e.NewValue.ToString()) != int.MaxValue;
            nud.btnDecrement.IsEnabled = int.Parse(e.NewValue.ToString()) != int.MinValue;
        }

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(WinHueIntegerUpDown), new FrameworkPropertyMetadata((int)1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void TbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbValue.Text == "" && CanBeEmpty)
            {
                Value = null;
                return;
            }
  

            if (!int.TryParse(tbValue.Text, out int val))
            {
                if (val > Max || val < Min)
                {
                    btnDecrement.IsEnabled = false;
                    btnIncrement.IsEnabled = false;
                }
            }
            else
            {
                Value = val;
            }
        }



        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(WinHueIntegerUpDown), new PropertyMetadata(int.MaxValue));



        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(WinHueIntegerUpDown), new PropertyMetadata(int.MinValue));



        public bool CanBeEmpty
        {
            get { return (bool)GetValue(CanBeEmptyProperty); }
            set { SetValue(CanBeEmptyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeEmptyProperty =
            DependencyProperty.Register("CanBeEmpty", typeof(bool), typeof(WinHueIntegerUpDown), new PropertyMetadata(false));




        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            int val = 0;
            if (tbValue.Text == "" && CanBeEmpty) return;
            if(int.TryParse(tbValue.Text,out val))
            {
                if (val > Max)
                {
                    tbValue.Text = Max.ToString();
                }

                if(val < Min)
                {
                    tbValue.Text = Min.ToString();
                }

                Value = int.Parse(tbValue.Text);
            }
            else
            {
                if (tbValue.Text == "")
                {
                    tbValue.Text = Min.ToString();
                }
                else
                {
                    if (tbValue.Text.Contains("-"))
                    {
                        tbValue.Text = Min.ToString();
                    }
                    else
                    {
                        tbValue.Text = Max.ToString();
                    }

                }
                Value = int.Parse(tbValue.Text);
            }

            btnDecrement.IsEnabled = true;
            btnIncrement.IsEnabled = true;


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
