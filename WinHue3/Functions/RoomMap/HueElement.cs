using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;


namespace WinHue3.Functions.RoomMap
{
    public class HueElement : Image
    {
    //    private object movingObject;
    //    private double firstXPos, firstYPos;
    //    private int ButtonSize = 50;

    //    public HueElement() : base()
    //    {
    //        this.PreviewMouseLeftButtonDown += HueElement_PreviewMouseLeftButtonDown;
    //        this.PreviewMouseLeftButtonUp += HueElement_PreviewMouseLeftButtonUp;
    //        this.PreviewMouseMove += HueElement_PreviewMouseMove;
    //    }

    //    private void HueElement_PreviewMouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject) {
    //            HueElement img = sender as HueElement;
    //            Canvas canvas = img.Parent as Canvas;

    //            double newLeft = e.GetPosition(canvas).X - firstXPos - canvas.Margin.Left;
    //            // newLeft inside canvas right-border?
    //            if (newLeft > canvas.Margin.Left + canvas.ActualWidth - img.ActualWidth)
    //                newLeft = canvas.Margin.Left + canvas.ActualWidth - img.ActualWidth;
    //            // newLeft inside canvas left-border?
    //            else if (newLeft < canvas.Margin.Left)
    //                newLeft = canvas.Margin.Left;
    //            img.SetValue(Canvas.LeftProperty, newLeft);

    //            double newTop = e.GetPosition(canvas).Y - firstYPos - canvas.Margin.Top;
    //            // newTop inside canvas bottom-border?
    //            if (newTop > canvas.Margin.Top + canvas.ActualHeight - img.ActualHeight)
    //                newTop = canvas.Margin.Top + canvas.ActualHeight - img.ActualHeight;
    //            // newTop inside canvas top-border?
    //            else if (newTop < canvas.Margin.Top)
    //                newTop = canvas.Margin.Top;
    //            img.SetValue(Canvas.TopProperty, newTop);
    //        }
    //    }

    //    private void HueElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    //    {
    //        HueElement img = sender as HueElement;
    //        Canvas canvas = img.Parent as Canvas;

    //        movingObject = null;

    //        // Put the image currently being dragged on top of the others
    //        int top = Canvas.GetZIndex(img);
    //        foreach (HueElement child in canvas.Children)
    //            if (top > Canvas.GetZIndex(child))
    //                top = Canvas.GetZIndex(child);
    //        Canvas.SetZIndex(img, top + 1);
    //    }

    //    private void HueElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        // In this event, we get the current mouse position on the control to use it in the MouseMove event.
    //        HueElement img = sender as HueElement;
    //        Canvas canvas = img.Parent as Canvas;

    //        firstXPos = e.GetPosition(img).X;
    //        firstYPos = e.GetPosition(img).Y;

    //        movingObject = sender;

    //        // Put the image currently being dragged on top of the others
    //        int top = Canvas.GetZIndex(img);
    //        foreach (HueElement child in canvas.Children)
    //            if (top < Canvas.GetZIndex(child))
    //                top = Canvas.GetZIndex(child);
    //        Canvas.SetZIndex(img, top + 1);
    //    }
    //
    }
}
