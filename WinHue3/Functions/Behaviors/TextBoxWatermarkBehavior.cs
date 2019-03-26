using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace WinHue3.Functions.Behaviors
{
    [Obsolete]
    public class TextBoxWatermarkBehavior : Behavior<TextBox>
    {
        private WaterMarkAdorner adorner;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxWatermarkBehavior), new PropertyMetadata("Watermark"));


        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(TextBoxWatermarkBehavior), new PropertyMetadata(12.0));


        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextBoxWatermarkBehavior), new PropertyMetadata(Brushes.Black));



        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(TextBoxWatermarkBehavior), new PropertyMetadata("Segoe UI"));


        public TextAlignment TextAlign
        {
            get { return (TextAlignment)GetValue(TextAlignProperty); }
            set { SetValue(TextAlignProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlign.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignProperty =
            DependencyProperty.Register("TextAlign", typeof(TextAlignment), typeof(TextBoxWatermarkBehavior), new PropertyMetadata(TextAlignment.Left));



        protected override void OnAttached()
        {
            adorner = new WaterMarkAdorner(this.AssociatedObject, this.Text, this.FontSize, this.FontFamily, this.Foreground, this.TextAlign);

            this.AssociatedObject.Loaded += this.OnLoaded;
            this.AssociatedObject.GotFocus += this.OnFocus;
            this.AssociatedObject.LostFocus += this.OnLostFocus;
            this.AssociatedObject.TextChanged += this.OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RemoveWatermark();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AddWatermark();
        }

        private void AddWatermark()
        {
            if (!this.AssociatedObject.IsFocused)
            {
                if (string.IsNullOrEmpty(this.AssociatedObject.Text))
                {
                    var layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
                    if (layer == null) return;
                    Adorner[] adorners = layer.GetAdorners(this.AssociatedObject);
                    
                    if (adorners != null && adorners.Any(x => x == adorner))
                    {
                        return;
                    }

                    layer.Add(adorner);

                }
            }
        }

        private void RemoveWatermark()
        {

            if (!string.IsNullOrEmpty(this.AssociatedObject.Text))
            {
                var layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
                if (layer == null) return;
                layer.Remove(adorner);
            }


        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            AddWatermark();
            
        }

        private void OnFocus(object sender, RoutedEventArgs e)
        {
            RemoveWatermark();
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
            private TextAlignment textalign;

            public WaterMarkAdorner(UIElement element, string text, double fontsize, string font, Brush foreground, TextAlignment align): base(element)
            {
                this.IsHitTestVisible = false;
                this.Opacity = 0.6;
                this.text = text;
                this.fontSize = fontsize;
                this.fontFamily = font;
                this.foreground = foreground;
                this.textalign = align;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                var text = new FormattedText(
                        this.text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(fontFamily),
                        fontSize,
                        foreground);
                Point textpos;

                switch (textalign)
                {

                    case TextAlignment.Right:
                        textpos = new Point(this.ActualWidth - text.WidthIncludingTrailingWhitespace - 5, 2);
                        break;
                    case TextAlignment.Center:
                        textpos = new Point(this.ActualWidth /2 - text.WidthIncludingTrailingWhitespace / 2,2);
                        break;
                    default:
                        textpos = new Point(5 , 2);
                        break;
                
                }
                drawingContext.DrawText(text, textpos);
            }
        }
    }

}
