using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models.AppSettings
{
    public class AppMainSettingsModel : ValidatableBindableBase
    {
        private bool detectProxy;
        private bool debug;
        private bool showHidden;
        private int upnpTimeout;
        private int timeout;
        private bool startWindows;
        private string language;
        private int startMode;

        public AppMainSettingsModel()
        {
            DetectProxy = false;
            Debug = true;
            ShowHidden = false;
            upnpTimeout = 5000;
            Timeout = 3000;
            Language = "en-US";
            StartMode = 0;
        }

        public bool DetectProxy
        {
            get { return detectProxy; }
            set { SetProperty(ref detectProxy,value); }
        }

        public bool Debug
        {
            get { return debug; }
            set { SetProperty(ref debug,value); }
        }

        public bool ShowHidden
        {
            get { return showHidden; }
            set { SetProperty(ref showHidden,value); }
        }

        public int UpnpTimeout
        {
            get { return upnpTimeout; }
            set { SetProperty(ref upnpTimeout,value); }
        }

        public int Timeout
        {
            get { return timeout; }
            set { SetProperty(ref timeout,value); }
        }

        public string Language
        {
            get { return language; }
            set { SetProperty(ref language,value); }
        }

        public int StartMode
        {
            get { return startMode; }
            set { SetProperty(ref startMode,value); }
        }
    }
}
