using System;
using System.Collections.ObjectModel;
using System.Windows;
using log4net.Appender;
using log4net.Core;

namespace WinHue3
{
    public class DataGridViewAppender : AppenderSkeleton
    {
        public ObservableCollection<DgLogEntry> DgEventLog;

        protected override void Append(LoggingEvent loggingEvent)
        {
            DgLogEntry newLogEntry = new DgLogEntry();
            newLogEntry.evdatetime = loggingEvent.TimeStamp;
            newLogEntry.level = loggingEvent.Level.ToString();
            newLogEntry.logger = loggingEvent.LoggerName;
            newLogEntry.message = loggingEvent.MessageObject.ToString();
            newLogEntry.method = loggingEvent.LocationInformation.MethodName;
            newLogEntry.line = loggingEvent.LocationInformation.LineNumber;
            newLogEntry.classname = loggingEvent.LocationInformation.ClassName;
            newLogEntry.thread = loggingEvent.ThreadName;

            Application.Current?.Dispatcher?.BeginInvoke(new Action(() => this.DgEventLog.Add(newLogEntry)));


        }

    }
}
