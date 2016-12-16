using System.ComponentModel;
using System.Windows;
using log4net;

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
