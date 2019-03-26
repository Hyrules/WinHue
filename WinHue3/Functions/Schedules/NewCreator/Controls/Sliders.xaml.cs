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

        public double HueValue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("HueValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HuePropertyChangedCallback));

        private static void HuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlHue.Value = Convert.ToDouble(e.NewValue);
        }

        public double BriValue
        {
            get => (double)GetValue(BriProperty);
            set => SetValue(BriProperty, value);
        }

        // Using a DependencyProperty as the backing store for Bri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BriProperty =
            DependencyProperty.Register("BriValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BriPropertyChangedCallback));

        private static void BriPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlBri.Value = Convert.ToDouble(e.NewValue);
        }

        public double SatValue
        {
            get => (double)GetValue(SatProperty);
            set => SetValue(SatProperty, value);
        }

        // Using a DependencyProperty as the backing store for Sat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SatProperty =
            DependencyProperty.Register("SatValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SatPropertyChangedCallback));

        private static void SatPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlSat.Value = Convert.ToDouble(e.NewValue);
        }

        public double CtValue
        {
            get => (double)GetValue(CtProperty);
            set => SetValue(CtProperty, value);
        }

        // Using a DependencyProperty as the backing store for Ct.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CtProperty =
            DependencyProperty.Register("CtValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(152d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CtPropertyChangedCallback));

        private static void CtPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlCt.Value = Convert.ToDouble(e.NewValue);
        }

        public double XValue
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("XValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(-0.001d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XPropertyChangedCallback));

        private static void XPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlX.Value = Convert.ToDouble(e.NewValue);
        }

        public double YValue
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("YValue", typeof(double), typeof(Sliders), new FrameworkPropertyMetadata(-0.001d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YPropertyChangedCallback));

        private static void YPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.SlY.Value = Convert.ToDouble(e.NewValue);
        }

        public ushort? TTValue
        {
            get => (ushort?)GetValue(TransitionTimeProperty);
            set => SetValue(TransitionTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransitionTimeProperty =
            DependencyProperty.Register("TTValue", typeof(ushort?), typeof(Sliders), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string HueEffect
        {
            get { return (string)GetValue(HueEffectProperty); }
            set { SetValue(HueEffectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HueEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueEffectProperty =
            DependencyProperty.Register("HueEffect", typeof(string), typeof(Sliders), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EffectPropertyChangedCallback));

        private static void EffectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.wcbEffect.SelectedValue = e.NewValue;
        }

        public string HueAlert
        {
            get { return (string)GetValue(HueAlertProperty); }
            set { SetValue(HueAlertProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HueAlert.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueAlertProperty =
            DependencyProperty.Register("HueAlert", typeof(string), typeof(Sliders), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AlertPropertyChangedCallback));

        private static void AlertPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.wcbAlert.SelectedValue = e.NewValue;
        }

        public bool? On
        {
            get { return (bool?)GetValue(OnProperty); }
            set { SetValue(OnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for On.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnProperty =
            DependencyProperty.Register("On", typeof(bool?), typeof(Sliders), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChangedCallback));

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sliders sliders = d as Sliders;
            sliders.chbOn.IsChecked = (bool?)e.NewValue;
        }
    }






}
