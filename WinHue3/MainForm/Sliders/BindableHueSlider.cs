using System.Windows;
using WinHue3.Colors;

namespace WinHue3.MainForm.Sliders
{
    public class BindableHueSlider : BindableSlider
    {

        public System.Windows.Media.Color Hue
        {
            get => (System.Windows.Media.Color)GetValue(HueProperty); set => SetValue(HueProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(System.Windows.Media.Color), typeof(BindableHueSlider), new PropertyMetadata(System.Windows.Media.Color.FromRgb(255,0,0)));

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Hue = new HSLColor(newValue / 273.06, 240, 120);
        }

    }
}
