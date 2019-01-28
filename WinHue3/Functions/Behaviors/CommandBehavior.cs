using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.Functions.Behaviors
{

    public static class CommandBehavior
    {
        private static DependencyObject obj;

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty); 
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandBehavior), new FrameworkPropertyMetadata((ICommand)null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            obj = d;
            if ((e.NewValue != null) && (e.OldValue == null))
            {
                ((ICommand)e.NewValue).CanExecuteChanged += OnCanExecuteChanged;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                ((ICommand) e.OldValue).CanExecuteChanged -= OnCanExecuteChanged;
            }
        }

        private static void OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (obj == null) return;
            if (!(obj is Control ctrl)) return;
            ctrl.IsEnabled = ((ICommand) obj.GetValue(CommandProperty)).CanExecute(null);
        }


        public static readonly DependencyProperty RoutedEventNameProperty =
            DependencyProperty.RegisterAttached("RoutedEventName", typeof(string),
               typeof(CommandBehavior), new FrameworkPropertyMetadata(string.Empty, OnRoutedEventNameChanged));
 
        /// <summary>
        /// Gets the RoutedEventName property.  
        /// </summary>
        public static string GetRoutedEventName(DependencyObject d)
        {
            return (string) d.GetValue(RoutedEventNameProperty);
        }

        /// <summary>
        /// Sets the RoutedEventName property.  
        /// </summary>
        public static void SetRoutedEventName(DependencyObject d, string value)
        {
            d.SetValue(RoutedEventNameProperty, value);
        }

        /// <summary>
        /// Hooks up a Dynamically created EventHandler (by using the 
        /// <see cref="EventHooker">EventHooker</see> class) that when
        /// run will run the associated ICommand
        /// </summary>
        private static void OnRoutedEventNameChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            string routedEvent = (string)e.NewValue;
  
            //If the RoutedEvent string is not null, create a new
            //dynamically created EventHandler that when run will execute
            //the actual bound ICommand instance (usually in the ViewModel)
            if (!string.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker {ObjectWithAttachedCommand = d};
                EventInfo eventInfo = d.GetType().GetEvent(routedEvent,BindingFlags.Public | BindingFlags.Instance);

                //Hook up Dynamically created event handler
                if (eventInfo != null)
                {
                     eventInfo.AddEventHandler(d,eventHooker.GetNewEventHandlerToRunCommand(eventInfo));
                }
            }
        }
    }

    /// <summary>
    /// Contains the event that is hooked into the source RoutedEvent
    /// that was specified to run the ICommand
    /// </summary>
    sealed class EventHooker
    {
        
        /// <summary>
        /// The DependencyObject, that holds a binding to the actual
        /// ICommand to execute
        /// </summary>
        public DependencyObject ObjectWithAttachedCommand { get; set; }
          
        /// <summary>
        /// Creates a Dynamic EventHandler that will be run the ICommand
        /// when the user specified RoutedEvent fires
        /// </summary>
        /// <param name="eventInfo">The specified RoutedEvent EventInfo</param>
        /// <returns>An Delegate that points to a new EventHandler
        /// that will be run the ICommand</returns>
        public Delegate GetNewEventHandlerToRunCommand(EventInfo eventInfo)
        {
            Delegate del = null;
  
            if (eventInfo == null)
                throw new ArgumentNullException("eventInfo");
  
            if (eventInfo.EventHandlerType == null)
                throw new ArgumentException("EventHandlerType is null");
  
            if (del == null)
                del = Delegate.CreateDelegate(eventInfo.EventHandlerType, this,GetType().GetMethod("OnEventRaised",BindingFlags.NonPublic | BindingFlags.Instance));
            return del;
        }

        /// <summary>
        /// Runs the ICommand when the requested RoutedEvent fires
        /// </summary>
        private void OnEventRaised(object sender, EventArgs e)
        {
            ICommand command = (ICommand)(sender as DependencyObject).GetValue(CommandBehavior.CommandProperty);
            if (command != null)
            {
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }

        }
    }
}
