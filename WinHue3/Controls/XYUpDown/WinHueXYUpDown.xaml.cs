using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class WinHueXYUpDown : UserControl
    {
        public WinHueXYUpDown()
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
                TbValue.Text = 0.ToString();
            }
            else
            {
                if (decimal.Parse(TbValue.Text) < 1 && decimal.Parse(TbValue.Text) > 0 )
                {
                    TbValue.Text = (decimal.Parse(TbValue.Text) + Step).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    TbValue.Text = 0.ToString();
                }
            }
            
            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnIncrement_Click(object sender, RoutedEventArgs e)
        {
            IncrementValue();            
        }

        public void DecrementValue()
        {

            if (string.IsNullOrEmpty(TbValue.Text))
            {
                TbValue.Text = decimal.MinValue.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                if (decimal.Parse(TbValue.Text) > 0 && decimal.Parse(TbValue.Text) < 1)
                {
                    TbValue.Text = (decimal.Parse(TbValue.Text) - Step).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    TbValue.Text = 1.ToString();
                }
            }

            CheckValues();
            SetDirtyTextBox();
        }

        private void BtnDecrement_Click(object sender, RoutedEventArgs e)
        {
            DecrementValue();            
        }

        public decimal? Value
        {
            get => (decimal?)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                ValueChanged?.Invoke(this,null);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(WinHueXYUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null , CoerceValue));

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            WinHueXYUpDown nud = d as WinHueXYUpDown;
            nud.ValueChanged?.Invoke(nud, EventArgs.Empty);
            if (basevalue == null)
            {
                if (nud.CanBeNull || nud.IsFocused)
                {
                    nud.TbValue.Text = "";
                    return basevalue;
                }
            }
            nud.TbValue.Text = $"{basevalue:0.000}";
            nud.SetIncrementalsButtons();
            return basevalue;
        }


        public decimal Step
        {
            get => (decimal)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(decimal), typeof(WinHueXYUpDown), new FrameworkPropertyMetadata((decimal)0.001,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool CanBeNull
        {
            get => (bool)GetValue(CanBeNullProperty);
            set => SetValue(CanBeNullProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanBeEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBeNullProperty =
            DependencyProperty.Register("CanBeNull", typeof(bool), typeof(WinHueXYUpDown), new FrameworkPropertyMetadata(true));


        private void SetIncrementalsButtons()
        {
            if (TbValue.Text == "" & CanBeNull)
            {
                BtnDecrement.IsEnabled = true;
                BtnIncrement.IsEnabled = true;
                return;
            }

            if (!decimal.TryParse(TbValue.Text, out _))
            {
                BtnDecrement.IsEnabled = false;
                BtnIncrement.IsEnabled = false;
                return;
            }
            BtnDecrement.IsEnabled = Value > 0;
            BtnIncrement.IsEnabled = Value < 1;
        }

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
                        Value = 0;
                }
            }
            else
            {
                Value = decimal.TryParse(TbValue.Text, out decimal val) ? val : 1;
            }

            SetIncrementalsButtons();
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckValues();
        }

        private void TbValue_OnPreviewKeyUp(object sender, KeyEventArgs e)
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
                    if (decimal.TryParse(TbValue.Text, out decimal val))
                    {
                        if (val >= 0 && val <= 1)
                        {
                            EnterPressed?.Invoke(this, EventArgs.Empty);
                            ClearDirtyTextBox();
                            return;
                        }
                    }
                    break;
            }
            CheckValues();
        }

        private void TbValue_OnPreviewKeyDown(object sender, KeyEventArgs e)
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
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) && e.Key != Key.OemComma && e.Key != Key.OemPeriod)
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
