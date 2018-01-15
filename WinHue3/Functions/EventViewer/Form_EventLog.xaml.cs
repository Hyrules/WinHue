using System.ComponentModel;
using System.Windows;
using log4net;

namespace WinHue3.Functions.EventViewer
{
    /// <summary>
    /// Interaction logic for Form_EventLog.xaml
    /// </summary>
    public partial class Form_EventLog : Window
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public readonly EventViewerViewModel ViewModel;
        public Form_EventLog()
        {
            InitializeComponent();
            ViewModel = DataContext as EventViewerViewModel;
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
