using System.Net;

namespace WinHue3.Functions.BridgeFinder
{
    public class ProgressReport
    {
        private IPAddress _ip;
        private int _progress;

        public ProgressReport(IPAddress ip, int progress)
        {
            Ip = ip;
            Progress = progress;
        }

        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }
    }
}