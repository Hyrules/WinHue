using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace WinHue3.Functions.Behaviors
{
    [Obsolete]
    public class ComboBoxWatermarkBehavior : Behavior<ComboBox>
    {
        private WaterMarkAdorner adorner;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ComboBoxWatermarkBehavior), new PropertyMetadata("Watermark"));


        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(ComboBoxWatermarkBehavior), new PropertyMetadata(12.0));


        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(ComboBoxWatermarkBehavior), new PropertyMetadata(Brushes.Black));



        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(ComboBoxWatermarkBehavior), new PropertyMetadata("Segoe UI"));



        protected override void OnAttached()
        {
            adorner = new WaterMarkAdorner(this.AssociatedObject, this.Text, this.FontSize, this.FontFamily, this.Foreground);
            this.AssociatedObject.Loaded += this.OnLoaded;
            this.AssociatedObject.GotFocus += this.OnFocus;
            this.AssociatedObject.LostFocus += this.OnLostFocus;
        }



        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.AssociatedObject.IsFocused)
            {
                if (string.IsNullOrEmpty(this.AssociatedObject.Text))
                {
                    var layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
                    if (layer == null) return;
                    layer.Add(adorner);
                }
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.AssociatedObject.Text))
            {
                try
                {
                    var layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
                    layer.Add(adorner);
                }
                catch { }
            }
        }

        private void OnFocus(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
            layer.Remove(adorner);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private class WaterMarkAdorner : Adorner
        {
            private string text;
            private double fontSize;
            private string fontFamily;
            private Brush foreground;

            public WaterMarkAdorner(UIElement element, string text, double fontsize, string font, Brush foreground)
                : base(element)
            {
                this.IsHitTestVisible = false;
                this.Opacity = 0.6;
                this.text = text;
                this.fontSize = fontsize;
                this.fontFamily = font;
                this.foreground = foreground;
            }

            protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                var text = new FormattedText(
                        this.text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new System.Windows.Media.Typeface(fontFamily),
                        fontSize,
                        foreground);
                
                drawingContext.DrawText(text, new Point(5, 3));
            }
        }
    }
}
