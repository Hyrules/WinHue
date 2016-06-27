using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WinHue3
{
    public class WaitView : View
    {
        private readonly DispatcherTimer _timer;
        private DispatcherTimer _pbtimer;
        private readonly string _message;
        private readonly TimeSpan _waittime;
        private int _pbvalue;
         
        public WaitView(string message, TimeSpan waittime)
        {
            _message = message;
            _waittime = waittime;
            _timer = new DispatcherTimer() {Interval = _waittime};
            _pbtimer = new DispatcherTimer() {Interval = new TimeSpan(0,0,0,1)};
            _timer.Tick += _timer_Tick;
            _pbtimer.Tick += _pbtimer_Tick;
            _pbvalue = 0;
           
        }

        private void _pbtimer_Tick(object sender, EventArgs e)
        {
            pbValue++;        
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            OnWaitComplete?.Invoke(sender,e);
        }

        public string Message => _message;
        public double WaitTime => _waittime.TotalSeconds;

        public int pbValue
        {
            get { return _pbvalue; }
            set { _pbvalue = value; OnPropertyChanged(); }
        }
        public event WaitComplete OnWaitComplete;
        public delegate void WaitComplete(object sender, EventArgs e);

        public void StartWait()
        {
            pbValue = 0;
            _timer.Start();
            _pbtimer.Start();
        }

        public void ResumeWait()
        {
            _timer.Start();
            _pbtimer.Start();            
        }

        public void StopWait()
        {
            _timer.Stop();
            _pbtimer.Stop();
        }

    }
}
