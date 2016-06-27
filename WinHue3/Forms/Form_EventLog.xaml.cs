using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication.ExtendedProtection;
using System.Windows;
using System.Windows.Controls;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System.Reflection;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_EventLog.xaml
    /// </summary>
    public partial class Form_EventLog : Window
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public EventViewerView evv;    

        public Form_EventLog()
        {
            evv = new EventViewerView();
            DataContext = evv;
            InitializeComponent();
        }

        
        /// <summary>
        /// Return the RichTextBox of the form.
        /// </summary>

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


    }

}
