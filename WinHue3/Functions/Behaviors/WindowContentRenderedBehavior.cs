using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace WinHue3.Functions.Behaviors
{
    public static class WindowContentRenderedBehavior
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(WindowsContentRenderedProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(WindowsContentRenderedProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowsContentRenderedProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(WindowContentRenderedBehavior), new FrameworkPropertyMetadata((ICommand)null,OnWindowsContentRenderedChanged));

        private static void OnWindowsContentRenderedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MetroWindow window)) return;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                window.ContentRendered += OnWindowContentRendered;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                window.ContentRendered -= OnWindowContentRendered;
            }
        }

        private static void OnWindowContentRendered(object sender, EventArgs e)
        {

            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(WindowsContentRenderedProperty);
            command.Execute(null);

        }
    }
}
