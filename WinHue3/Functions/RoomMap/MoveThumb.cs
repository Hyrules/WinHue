using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Controls;


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

            HueElement he = (HueElement) item.DataContext;
            he.X += e.HorizontalChange;
            he.Y += e.VerticalChange;
      
            Canvas canvas =  item.FindVisualAncestor<Canvas>();

            if (he.X < 0)
                he.X = 0;

            if (he.Y < 0)
                he.Y = 0;

            if (he.X + this.ActualWidth > canvas.ActualWidth)
                he.X = canvas.ActualWidth - this.ActualWidth;


            if (he.Y + this.ActualHeight > canvas.ActualHeight)
                he.Y = canvas.ActualHeight - this.ActualHeight;

            //if (Canvas.GetLeft(item) < 0)
            //    Canvas.SetLeft(item, 0);

            //if (Canvas.GetLeft(item) + this.ActualWidth > canvas.ActualWidth)
            //    Canvas.SetLeft(item, canvas.ActualWidth - this.ActualWidth);

            //if (Canvas.GetTop(item) < 0)
            //    Canvas.SetTop(item, 0);

            //if (Canvas.GetTop(item) + this.ActualHeight > canvas.ActualHeight)
            //    Canvas.SetTop(item, canvas.ActualHeight - this.ActualHeight);


        }
    }
}
