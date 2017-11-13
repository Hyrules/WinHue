using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace WinHue3.Functions.Schedules.NewCreator.Controls
{
    /// <summary>
    /// Interaction logic for Sliders.xaml
    /// </summary>
    public partial class Sliders : UserControl
    {
        public Sliders()
        {
            InitializeComponent();
        }

        public int Hue
        {
            get { return (int)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HuePropertyChangedCallback));

        private static void HuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slHue.Value = Convert.ToDouble(e.NewValue);
        }

        public int Bri
        {
            get { return (int)GetValue(BriProperty); }
            set { SetValue(BriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BriProperty =
            DependencyProperty.Register("Bri", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BriPropertyChangedCallback));

        private static void BriPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slBri.Value = Convert.ToDouble(e.NewValue);
        }

        public int Sat
        {
            get { return (int)GetValue(SatProperty); }
            set { SetValue(SatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SatProperty =
            DependencyProperty.Register("Sat", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SatPropertyChangedCallback));

        private static void SatPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slSAT.Value = Convert.ToDouble(e.NewValue);
        }

        public int Ct
        {
            get { return (int)GetValue(CtProperty); }
            set { SetValue(CtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ct.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CtProperty =
            DependencyProperty.Register("Ct", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CtPropertyChangedCallback));

        private static void CtPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slCT.Value = Convert.ToDouble(e.NewValue);
        }

        public int X
        {
            get { return (int)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XPropertyChangedCallback));

        private static void XPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slX.Value = Convert.ToDouble(e.NewValue);
        }

        public int Y
        {
            get { return (int)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YPropertyChangedCallback));

        private static void YPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.slY.Value = Convert.ToDouble(e.NewValue);
        }
    }
}
