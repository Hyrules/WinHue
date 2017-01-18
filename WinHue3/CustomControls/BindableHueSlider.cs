using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace WinHue3.CustomControls
{
    public class BindableHueSlider : BindableSlider
    {


        public Color Hue
        {
            get { return (Color)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(Color), typeof(BindableHueSlider), new PropertyMetadata(Color.FromRgb(255,0,0)));

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Hue = new HSLColor() { Hue = newValue / ushort.MaxValue };
        }
    }
}
