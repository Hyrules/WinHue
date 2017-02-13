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

namespace WinHue3.Controls
{
    /// <summary>
    /// Interaction logic for WatermarkTextBox.xaml
    /// </summary>
    public partial class WatermarkTextBox : UserControl
    {
        public WatermarkTextBox()
        {
            InitializeComponent();
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new FrameworkPropertyMetadata("Please enter name", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChanged));

        private static void PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            WatermarkTextBox wtb = dependencyObject as WatermarkTextBox;
            wtb.tbWatermark.Text = wtb.Watermark;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextPropertyChanged));

        private static void TextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            WatermarkTextBox wtb = dependencyObject as WatermarkTextBox;
            wtb.tbWatermark.Background = (string)dependencyPropertyChangedEventArgs.NewValue != string.Empty ? new SolidColorBrush(Colors.White) : null;
        }

        private void tbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = tbValue.Text;
        }

        private void tbValue_GotFocus(object sender, RoutedEventArgs e)
        {
            tbValue.Background = new SolidColorBrush(Colors.White);
        }

        private void tbValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbValue.Text == string.Empty)
                tbValue.Background = null;
        }
    }
}
