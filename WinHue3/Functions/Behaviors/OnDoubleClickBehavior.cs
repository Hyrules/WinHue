using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.Functions.Behaviors
{
    public static class OnDoubleClickBehavior
    {
        private static DependencyObject obj;

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(OnDoubleClickProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OnDoubleClickProperty,value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnDoubleClickProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(OnDoubleClickBehavior), new FrameworkPropertyMetadata(OnDoubleClickPropertyChanged));


        public static void SetIgnoreDblClick_CanExecute(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreDblClickCanExecuteProperty,value);
        }

        public static bool GetIgnoreDblClick_CanExecute(DependencyObject obj)
        {
            return (bool) obj.GetValue(IgnoreDblClickCanExecuteProperty);
        }

        public static readonly DependencyProperty IgnoreDblClickCanExecuteProperty =
            DependencyProperty.RegisterAttached("IgnoreDblClick_CanExecute", typeof(bool), typeof(OnDoubleClickBehavior), new FrameworkPropertyMetadata(false));


        private static void OnDoubleClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            if(!(d is Control ctrl)) return;
            obj = d;
            if ((e.NewValue != null) && (e.OldValue == null))
            {
                ctrl.MouseDoubleClick += OnMouseDoubleClick;
                ((ICommand)e.NewValue).CanExecuteChanged += OnCanExecuteChanged;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                ctrl.MouseDoubleClick -= OnMouseDoubleClick;
                ((ICommand)e.OldValue).CanExecuteChanged -= OnCanExecuteChanged;
            }
        }

        private static void OnCanExecuteChanged(object sender, EventArgs e)
        {            
            if (!(obj is Control ctrl)) return;
            bool ignore = (bool) obj.GetValue(IgnoreDblClickCanExecuteProperty);
            if (ignore) return;
            ICommand command = (ICommand)obj.GetValue(OnDoubleClickProperty);
            if (command.CanExecute(null) != ctrl.IsEnabled)
                ctrl.IsEnabled = command.CanExecute(null);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(OnDoubleClickProperty);
            if(command.CanExecute(null))
                command.Execute(null);
        }
    }
}
