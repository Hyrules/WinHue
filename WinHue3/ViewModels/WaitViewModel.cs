using System;
using System.Windows.Threading;

namespace WinHue3.ViewModels
{
    public class WaitViewModel : ValidatableBindableBase
    {
        private readonly DispatcherTimer _timer;
        private DispatcherTimer _pbtimer;
        private string _message;
        private TimeSpan _waittime;
        private int _pbvalue;
         
        public WaitViewModel()
        {
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

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }


        public TimeSpan WaitTime
        {
            get => _waittime;
            set
            {
                SetProperty(ref _waittime, value);
                _pbtimer.Interval = _waittime;              
            }
        }

        public int pbValue
        {
            get => _pbvalue;
            set => SetProperty(ref _pbvalue,value);
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
