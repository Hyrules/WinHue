using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.Functions.Behaviors
{
    public static class OnClickBehavior
    {
        private static DependencyObject obj;

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OnClickCommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OnClickCommandProperty,value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnClickCommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(OnClickBehavior), new FrameworkPropertyMetadata(OnClickCommandPropertyChanged));


        public static void SetIgnoreClick_CanExecute(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreClickCanExecuteProperty, value);
        }

        public static bool GetIgnoreClick_CanExecute(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreClickCanExecuteProperty);
        }

        public static readonly DependencyProperty IgnoreClickCanExecuteProperty =
            DependencyProperty.RegisterAttached("IgnoreClick_CanExecute", typeof(bool), typeof(OnDoubleClickBehavior), new FrameworkPropertyMetadata(false));


        private static void OnClickCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Control ctrl)) return;
            obj = d;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                ctrl.PreviewMouseUp += OnMouseUp;
                ((ICommand)e.NewValue).CanExecuteChanged += OnCanExecuteChanged;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                ctrl.PreviewMouseUp -= OnMouseUp;
                ((ICommand) e.OldValue).CanExecuteChanged -= OnCanExecuteChanged;
            }

        }

        private static void OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (!(obj is Control ctrl)) return;
            bool ignore = (bool)obj.GetValue(IgnoreClickCanExecuteProperty);
            if (ignore) return;
            ICommand command = (ICommand) obj.GetValue(OnClickCommandProperty);
            if(command.CanExecute(null) != ctrl.IsEnabled)
                ctrl.IsEnabled = command.CanExecute(null);
        }

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(OnClickCommandProperty);
            if(command.CanExecute(null))
                command.Execute(null);
        }
    }
}
