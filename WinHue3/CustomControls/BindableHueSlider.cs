using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WinHue3.Annotations;


namespace WinHue3.CustomControls
{
    public class BindableHueSlider : BindableSlider, INotifyPropertyChanged
    {


        public Color Hue
        {
            get { return (Color)GetValue(HueProperty); }
            set
            {
                SetValue(HueProperty, value);
                OnPropertyChanged();
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(Color), typeof(BindableHueSlider), new PropertyMetadata(Color.FromRgb(255,0,0)));

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            int r = 0, g = 0, b = 0;
            ColorConversion.HSLToRgb(newValue / 182.042,1,1,out r,out g,out b);
            Color col = new Color
            {
                A = 255,
                R = (byte) r,
                G = (byte) g,
                B = (byte) b
            };
            Hue = col;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
