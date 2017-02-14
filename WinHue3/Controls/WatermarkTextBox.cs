using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WinHue3.Controls
{
    public class WatermarkTextBox : TextBox
    {
        private bool _displaywm = true;

        public WatermarkTextBox() : base()
        {
            //_displaywm = true;
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public new string Text
        {
            get
            {
                if (_displaywm)
                    return (string)GetValue(WatermarkProperty);
                else
                    return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(WatermarkProperty, value);
            }
        }

        public new static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TextChanged)));

        private new static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBox wtb = d as WatermarkTextBox;
            if ((string)e.NewValue != string.Empty)
                wtb._displaywm = false;
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new FrameworkPropertyMetadata(string.Empty,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(WatermarkChanged)));

        private static void WatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBox wtb = d as WatermarkTextBox;
           
        }
/*
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Text == string.Empty)
                _displaywm = true;
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Text == Watermark)
                _displaywm = false;
            base.OnGotFocus(e);
        }
*/

    }



}
