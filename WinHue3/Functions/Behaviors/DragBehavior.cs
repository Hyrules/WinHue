using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using WinHue3.Functions.RoomMap;
using Xceed.Wpf.AvalonDock.Controls;


namespace WinHue3.Functions.Behaviors
{
    public class DragBehavior : Behavior<UIElement>
    {
        private Point clickPosition;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseRightButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.PreviewMouseRightButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
            base.OnAttached();
        }


        private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                FrameworkElement fe = sender as FrameworkElement;
                HueElement he = fe.DataContext as HueElement;
                int delta = (int)Math.Round((double) e.Delta / 100);
                Console.WriteLine($"Height : {he.PanelHeight}, Width : {he.PanelWidth}");
                if((he.PanelHeight <= 48 && delta < 0) || (he.PanelHeight >= 120 && delta > 0) )
                {
                    delta = 0;
                    
                }
                Console.WriteLine(delta);
                he.PanelHeight += delta;
                he.PanelWidth += delta;
                he.ImageHeight += delta;
                he.ImageWidth += delta;


                if(he.PanelHeight < 48)
                {
                    he.Label = he.Id;
                }

                e.Handled = true;
            }
            
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (!AssociatedObject.IsMouseCaptured) return;
            FrameworkElement fe = sender as FrameworkElement;
            Canvas canvas = fe.FindVisualAncestor<Canvas>();    
            Point mousePosOnCanvas = e.GetPosition(canvas);

            HueElement he = fe.DataContext as HueElement;

            he.X = mousePosOnCanvas.X - clickPosition.X;
            he.Y = mousePosOnCanvas.Y - clickPosition.Y;

            if (he.X < 0)
                he.X = 0;
            if (he.Y < 0)
                he.Y = 0;

            if (he.X > canvas.ActualWidth - fe.ActualWidth)
                he.X = canvas.ActualWidth - fe.ActualWidth;

            if (he.Y > canvas.ActualHeight - fe.ActualHeight)
                he.Y = canvas.ActualHeight - fe.ActualHeight;

        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {                      
            AssociatedObject.CaptureMouse();
            clickPosition = e.GetPosition(AssociatedObject);
            e.Handled = true;


        }
    }
}
