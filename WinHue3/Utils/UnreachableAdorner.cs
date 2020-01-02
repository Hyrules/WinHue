using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Drawing;
using System.Windows.Media;

namespace WinHue3.Utils
{
    public class UnreachableAdorner : Adorner
    {
        public UnreachableAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            SolidColorBrush renderBrush = new SolidColorBrush(System.Windows.Media.Colors.Black) { Opacity = 0.1 };
        }
    }
}
