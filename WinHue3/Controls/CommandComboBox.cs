using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace WinHue3.Controls
{
    public class CommandComboBox : WatermarkComboBox, ICommandSource
    {

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }
        public static readonly DependencyProperty CommandTargetProperty =
        DependencyProperty.Register(
            "CommandTarget",
            typeof(IInputElement),
            typeof(CommandComboBox),
            new UIPropertyMetadata(null)
            );

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(CommandComboBox),
                new UIPropertyMetadata(null)
                );

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(CommandComboBox),
                new PropertyMetadata(null,
                CommandChanged
                ));

        // Command dependency property change callback.
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandComboBox cs = (CommandComboBox)d;
            cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property.
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command.
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += handler;
            }
        }

        [DebuggerStepThrough]
        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (Command != null)
            {

                // If a RoutedCommand.
                if (Command is RoutedCommand command)
                {
                    IsEnabled = command.CanExecute(CommandParameter, CommandTarget);
                }
                // If a not RoutedCommand.
                else
                {
                    IsEnabled = Command.CanExecute(CommandParameter);
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (Command == null) return;

            if (Command is RoutedCommand command)
            {
                command.Execute(CommandParameter, CommandTarget);
            }
            else
            {
                Command.Execute(CommandParameter);
            }
        }
    }

}