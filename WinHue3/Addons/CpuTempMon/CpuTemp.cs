using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using OpenHardwareMonitor.Hardware;
using WinHue3.ExtensionMethods;
using WinHue3.Utils;

namespace WinHue3.Addons.CpuTempMon
{
    public class CpuTemp : ValidatableBindableBase, IDisposable
    {
        private float? _temperature;
        private ObservableCollection<ISensor> _cpuSensors;
        private string _pollSensorName;
        private readonly DispatcherTimer timer;
        private readonly Computer comp;
        private readonly BackgroundWorker bgwOpen;
        private bool _working = false;
        private bool _disposed = false;

        public string PollSensorName
        {
            get => _pollSensorName;
            set => SetProperty(ref _pollSensorName, value);
        }

        public float? Temperature
        {
            get => _temperature;
            private set => SetProperty(ref _temperature, value);
        }

        public ObservableCollection<ISensor> CpuSensors
        {
            get => _cpuSensors;
            private set => SetProperty(ref _cpuSensors, value);
        }

        public bool Working 
        {
            get => _working;
            private set => SetProperty(ref _working,value);
        }

        public CpuTemp(int pollingInterval)
        {
            
            comp = new Computer();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, pollingInterval);
            comp.CPUEnabled = true;
            bgwOpen = new BackgroundWorker();
            bgwOpen.DoWork += BgwOpen_DoWork;
           
            _pollSensorName = "CPU Package";
        }

        void BgwOpen_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                comp.Open();
                timer.Start();
                Working = true;
            }
            catch(Exception)
            {
                Working = false;
            }
            

        }

        public void Start()
        {
            if (!timer.IsEnabled && !Working)
            {
                bgwOpen.RunWorkerAsync();
            }
        }

        public void Stop()
        {
            if (!timer.IsEnabled || !Working) return;
            timer.Stop();
            comp.Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateSensors();
            int cpuPackage = CpuSensors.FindIndex(sensor => sensor.Name == PollSensorName);
            if (cpuPackage == -1)
                cpuPackage = 0;
            
            Temperature = CpuSensors[cpuPackage].Value;
            OnTempUpdated?.Invoke(this, new CpuTempEventArgs() { currentTemp= Temperature});
        }

        public event CpuTempEvent OnTempUpdated;
        public delegate void CpuTempEvent(object sender, CpuTempEventArgs e);
        public event CpuSensorEvent OnSensorUpdated;
        public delegate void CpuSensorEvent(object sender, EventArgs e);

        private void UpdateSensors()
        {
            foreach (IHardware hardware in comp.Hardware)
            {
                if (hardware.HardwareType != HardwareType.CPU) continue;

                hardware.Update();
                List<ISensor> sensors = hardware.Sensors.ToList<ISensor>();
                sensors.RemoveAll(sens => sens.SensorType != SensorType.Temperature);
                CpuSensors = new ObservableCollection<ISensor>(sensors);
            }
            OnSensorUpdated?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Stop();
                    bgwOpen.Dispose();
                }

                _disposed = true;
            }
        }
    }

    public class CpuTempEventArgs : EventArgs
    {
        public float? currentTemp;
        
    }
}
