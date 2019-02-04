using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using log4net.Repository.Hierarchy;
using WinHue3.Logs;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Reflection;
using System.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using Form_EventLog = WinHue3.Functions.EventViewer.Form_EventLog;

namespace WinHue3
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Form_EventLog _fel = new Form_EventLog();

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (log4net.LogManager.GetRepository() is Hierarchy hier)
            {
                DataGridViewAppender dgva = (DataGridViewAppender)hier.GetAppenders().FirstOrDefault(appender => appender.Name.Equals("DataGridViewAppender"));
                dgva.DgEventLog = _fel.ViewModel.EventViewerModel.ListLogEntries;
            }

            Log.Info(WinHueSettings.settings.Language);
            if (!string.IsNullOrEmpty(WinHueSettings.settings.Language) &&
                !string.IsNullOrWhiteSpace(WinHueSettings.settings.Language))
            {
                var culture = new CultureInfo(WinHueSettings.settings.Language);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Info($@"WinHue {Assembly.GetExecutingAssembly().GetName().Version} started");
            Log.Info($"User is running as administrator : {UacHelper.IsProcessElevated()}");
            MainForm.MainWindow wnd = new MainForm.MainWindow(_fel);
         
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
                    wnd.Show();
                    wnd.WindowState = WindowState.Normal;
                    break;
                case 1:
                    wnd.Show();
                    wnd.WindowState = WindowState.Minimized;                   
                    break;
                case 2:
                    wnd.Show();
                    wnd.Hide();
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
            string ex = Serializer.SerializeJsonObject(e.ExceptionObject);
            Log.Fatal("Unexpected Exception : ",(Exception)e.ExceptionObject);
            Log.Fatal(ex);
        }


    }
}
