using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WinHue3.Functions.RoomMap
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(this.DataContext is Control item)) return;
            
            double left = Canvas.GetLeft(item);
            double top = Canvas.GetTop(item);
            
            Canvas.SetLeft(item, left + e.HorizontalChange);
            Canvas.SetTop(item, top + e.VerticalChange);
            

            Canvas canvas = item.Parent as Canvas;
            if (canvas == null)
                return;
            
            if (Canvas.GetLeft(item) < 0)
                Canvas.SetLeft(item, 0);

            if (Canvas.GetLeft(item) + this.ActualWidth > canvas.ActualWidth)
                Canvas.SetLeft(item, canvas.ActualWidth - this.ActualWidth);

            if (Canvas.GetTop(item) < 0)
                Canvas.SetTop(item, 0);

            if (Canvas.GetTop(item) + this.ActualHeight > canvas.ActualHeight)
                Canvas.SetTop(item, canvas.ActualHeight - this.ActualHeight);

            
        }
    }
}
