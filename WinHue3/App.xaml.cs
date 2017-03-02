using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Forms;
using HueLib2;
using log4net.Repository.Hierarchy;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace WinHue3
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string ver = "BETA 26";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Form_EventLog fel = new Form_EventLog();

        public App() : base()
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Hierarchy hier = log4net.LogManager.GetRepository() as Hierarchy;

            if (hier != null)
            {
                DataGridViewAppender dgva = (DataGridViewAppender)hier.GetAppenders().FirstOrDefault(appender => appender.Name.Equals("DataGridViewAppender"));
                dgva.DgEventLog = fel.ViewModel.EventViewerModel.ListLogEntries;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            log.Info($@"WinHue {ver} started");
            MainWindow wnd = new MainWindow(fel) { Version = ver};
            MainWindow.Title = "WinHue 3 " + ver;
            double height = SystemParameters.WorkArea.Height * 0.75 >= MainWindow.MinHeight
                ? SystemParameters.WorkArea.Height*0.75
                : MainWindow.MinHeight;

            double width = SystemParameters.WorkArea.Width * 0.75 >= MainWindow.MinWidth
                ? SystemParameters.WorkArea.Width * 0.75
                : MainWindow.MinWidth;

            MainWindow.Height = height;
            MainWindow.Width = width;

            switch (WinHueSettings.settings.StartMode)
            {
                case 0:
                    wnd.WindowState = WindowState.Normal;
                    wnd.Show();
                    break;
                case 1:
                    wnd.Hide();
                    break;
                case 2:
                    wnd.WindowState = WindowState.Minimized;
                    wnd.Show();
                    break;
                default:
                    wnd.Show();
                    wnd.WindowState = WindowState.Normal;
                    break;
            }

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
            MessageBox.Show("Sorry but an unexpected exception occured. Please report the exception on the support website so the developper can fix the issues. Please include the most recent log located in the logs folder.");
            string ex = Serializer.SerializeToJson(e.ExceptionObject);
            log.Fatal("Unexpected Exception : ",(Exception)e.ExceptionObject);
            log.Fatal(ex);
        }

    }
}
