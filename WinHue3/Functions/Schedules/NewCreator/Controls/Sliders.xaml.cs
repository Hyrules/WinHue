using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

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

        public int HueValue
        {
            get => (int)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("HueValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HuePropertyChangedCallback));

        private static void HuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlHue.Value = Convert.ToDouble(e.NewValue);
        }

        public int BriValue
        {
            get => (int)GetValue(BriProperty);
            set => SetValue(BriProperty, value);
        }

        // Using a DependencyProperty as the backing store for Bri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BriProperty =
            DependencyProperty.Register("BriValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BriPropertyChangedCallback));

        private static void BriPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlBri.Value = Convert.ToDouble(e.NewValue);
        }

        public int SatValue
        {
            get => (int)GetValue(SatProperty);
            set => SetValue(SatProperty, value);
        }

        // Using a DependencyProperty as the backing store for Sat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SatProperty =
            DependencyProperty.Register("SatValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SatPropertyChangedCallback));

        private static void SatPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlSat.Value = Convert.ToDouble(e.NewValue);
        }

        public int CtValue
        {
            get => (int)GetValue(CtProperty);
            set => SetValue(CtProperty, value);
        }

        // Using a DependencyProperty as the backing store for Ct.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CtProperty =
            DependencyProperty.Register("CtValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CtPropertyChangedCallback));

        private static void CtPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlCt.Value = Convert.ToDouble(e.NewValue);
        }

        public int XValue
        {
            get => (int)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("XValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XPropertyChangedCallback));

        private static void XPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlX.Value = Convert.ToDouble(e.NewValue);
        }

        public int YValue
        {
            get => (int)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("YValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YPropertyChangedCallback));

        private static void YPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlY.Value = Convert.ToDouble(e.NewValue);
        }

        public int TTValue
        {
            get => (int)GetValue(TransitionTimeProperty);
            set => SetValue(TransitionTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransitionTimeProperty =
            DependencyProperty.Register("TTValue", typeof(int), typeof(Sliders), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TtPropertyChangedCallback));

        private static void TtPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.iudTT.Value = Convert.ToInt32(e.NewValue);
        }

        private void SlHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HueValue = Convert.ToInt32(e.NewValue);
        }

        private void SlBri_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BriValue = Convert.ToInt32(e.NewValue);
        }

        private void SlCt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CtValue = Convert.ToInt32(e.NewValue);
        }

        private void SlSat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SatValue = Convert.ToInt32(e.NewValue);
        }

        private void SlX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            XValue = Convert.ToInt32(e.NewValue);
        }

        private void SlY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            YValue = Convert.ToInt32(e.NewValue);
        }

        private void iudTT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TTValue = Convert.ToInt32(e.NewValue);
        }
    }






}
