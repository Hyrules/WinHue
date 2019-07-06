using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace WinHue3.Functions.Error
{
    /// <summary>
    /// Interaction logic for Form_ErrorWindow.xaml
    /// </summary>
    public partial class Form_ErrorWindow : Window
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Form_ErrorWindow()
        {
            InitializeComponent();
            
        }

        public string ErrorMessage
        {
            set => tbErrorMessage.Text = value;
            get => tbErrorMessage.Text;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\WinHue\log");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
