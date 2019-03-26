using System.ComponentModel;
using System.Windows;

namespace WinHue3.Functions.EventViewer
{
    /// <summary>
    /// Interaction logic for Form_EventLog.xaml
    /// </summary>
    public partial class Form_EventLog : Window
    {
        
        public Form_EventLog()
        {
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
