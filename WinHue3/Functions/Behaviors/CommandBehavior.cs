using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WinHue3.Functions.Behaviors
{
    public static class CommandBehavior
    {

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
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandBehavior), new FrameworkPropertyMetadata((ICommand)null));

        public static readonly DependencyProperty RoutedEventNameProperty =
            DependencyProperty.RegisterAttached("RoutedEventName", typeof(String),
               typeof(CommandBehavior), new FrameworkPropertyMetadata((String) String.Empty, new PropertyChangedCallback(OnRoutedEventNameChanged)));
 
        /// <summary>
        /// Gets the RoutedEventName property.  
        /// </summary>
        public static String GetRoutedEventName(DependencyObject d)
        {
            return (String) d.GetValue(RoutedEventNameProperty);
        }

        /// <summary>
        /// Sets the RoutedEventName property.  
        /// </summary>
        public static void SetRoutedEventName(DependencyObject d, String value)
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
            String routedEvent = (String)e.NewValue;
  
            //If the RoutedEvent string is not null, create a new
            //dynamically created EventHandler that when run will execute
            //the actual bound ICommand instance (usually in the ViewModel)
            if (!String.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker();
                eventHooker.ObjectWithAttachedCommand = d;
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
                command.Execute(null);
            }
            
        }
    }
}
