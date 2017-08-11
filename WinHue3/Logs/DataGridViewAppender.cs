using System;
using System.Collections.ObjectModel;
using System.Windows;
using log4net.Appender;
using log4net.Core;

namespace WinHue3.Logs
{
    public class DataGridViewAppender : AppenderSkeleton
    {
        public ObservableCollection<DgLogEntry> DgEventLog;

        protected override void Append(LoggingEvent loggingEvent)
        {
            DgLogEntry newLogEntry = new DgLogEntry
            {
                evdatetime = loggingEvent.TimeStamp,
                level = loggingEvent.Level.ToString(),
                logger = loggingEvent.LoggerName,
                message = loggingEvent.MessageObject?.ToString(),
                method = loggingEvent.LocationInformation.MethodName,
                line = loggingEvent.LocationInformation.LineNumber,
                classname = loggingEvent.LocationInformation.ClassName,
                thread = loggingEvent.ThreadName
            };

            Application.Current?.Dispatcher?.BeginInvoke(new Action(() => this.DgEventLog.Add(newLogEntry)));


        }

    }
}
