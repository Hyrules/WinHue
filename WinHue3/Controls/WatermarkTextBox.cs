using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using FlowDirection = System.Windows.FlowDirection;
using TextBox = System.Windows.Controls.TextBox;

namespace WinHue3.Controls
{
    public class WatermarkTextBox : TextBox
    {
        public WatermarkTextBox() :base()
        {
            DefaultStyleKeyProperty.OverrideMetadata(Watermark.GetType(),new FrameworkPropertyMetadata(Watermark));
           
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(WatermarkChanged)));

        private static void WatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
         //   WatermarkTextBox wtb = d as WatermarkTextBox;
        //    wtb.Background = new DrawingBrush(wtb.DrawText(wtb.Watermark));
        }
/*
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.Text == string.Empty)
            {
                this.Background = new SolidColorBrush(Colors.White);
            }

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.Text == string.Empty)
            {
                this.Background = new DrawingBrush(DrawText(Watermark));
            }
            base.OnLostFocus(e);
        }
        */
        private Drawing DrawText(string textString)
        {
            // Create a new DrawingGroup of the control.
            DrawingGroup drawingGroup = new DrawingGroup();

            // Open the DrawingGroup in order to access the DrawingContext.
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = new FormattedText(
                    textString,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, 
                    new Typeface("Segoe UI"),
                    48,
                    System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text. 
                    );

                formattedText.SetFontSize(this.FontSize);
                formattedText.SetFontWeight(FontWeights.Normal);

                // Build the geometry object that represents the text.
                Geometry textGeometry = formattedText.BuildGeometry(new System.Windows.Point(Padding.Left, 0));

                double width = formattedText.Width + (this.Width - formattedText.Width) - Padding.Left - Padding.Right;
                double height = formattedText.Height + (this.Height - formattedText.Height) - Padding.Top - Padding.Bottom;
                // Draw a rounded rectangle under the text that is slightly larger than the text.
                drawingContext.DrawRoundedRectangle(System.Windows.Media.Brushes.White, null, new Rect(new System.Windows.Size(width, height)), 0, 0);

                // Draw the outline based on the properties that are set.
                drawingContext.DrawGeometry(System.Windows.Media.Brushes.Gray, new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gray, 0), textGeometry);

                // Return the updated DrawingGroup content to be used by the control.
                return drawingGroup;
            }
        }
    }
}
