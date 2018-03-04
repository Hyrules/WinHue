using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.Functions.Behaviors
{
    public static class KeyUpBehavior
    {
        private static DependencyObject obj;

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(KeyUpProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(KeyUpProperty,value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyUpProperty =
            DependencyProperty.RegisterAttached("Command", typeof(int), typeof(KeyUpBehavior), new FrameworkPropertyMetadata(OnKeyUpPropertyChanged));


        public static void SetIgnoreKeyUp_CanExecute(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreKeyUpCanExecuteProperty, value);
        }

        public static bool GetIgnoreKeyUp_CanExecute(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreKeyUpCanExecuteProperty);
        }

        public static readonly DependencyProperty IgnoreKeyUpCanExecuteProperty =
            DependencyProperty.RegisterAttached("IgnoreKeyUpCanExecute", typeof(bool), typeof(OnDoubleClickBehavior), new FrameworkPropertyMetadata(false));


        private static void OnKeyUpPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            obj = d;
            if (!(d is Control ctrl)) return;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                ctrl.PreviewKeyUp += OnKeyUp;
                ((ICommand)e.NewValue).CanExecuteChanged += OnCanExecuteChanged;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                ctrl.PreviewKeyUp -= OnKeyUp;
                ((ICommand)e.OldValue).CanExecuteChanged -= OnCanExecuteChanged;
            }
        }

        private static void OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (!(obj is Control ctrl)) return;
            bool ignore = (bool)obj.GetValue(IgnoreKeyUpCanExecuteProperty);
            if (ignore) return;
            ICommand command = (ICommand)obj.GetValue(KeyUpProperty);
            if (command.CanExecute(null) != ctrl.IsEnabled)
                ctrl.IsEnabled = command.CanExecute(null);
        }


        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                UIElement element = (UIElement)sender;
                ICommand command = (ICommand)element.GetValue(KeyUpProperty);
                if(command.CanExecute(null))
                    command.Execute(null);
            }
        }
    }
}
