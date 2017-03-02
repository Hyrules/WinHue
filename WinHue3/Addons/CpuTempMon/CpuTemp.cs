using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Collections;
using System.Windows.Threading;
using System.ComponentModel;

namespace WinHue3
{
    public class CpuTemp
    {
        public float? temperature
        {
            get;
            private set;
        }

        public string pollSensorName;

        public List<ISensor> cpuSensors
        {
            get;
            private set;
        }

        private readonly DispatcherTimer timer;
        private readonly Computer comp;
        private readonly BackgroundWorker bgwOpen;
        private bool _notWorking = false;

        public bool Working 
        {
            get{ return _notWorking;}
            private set{ _notWorking = value;}
        }

        public CpuTemp(int pollingInterval)
        {
            
            comp = new Computer();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, pollingInterval);
            comp.CPUEnabled = true;

            bgwOpen = new BackgroundWorker();
            bgwOpen.DoWork += bgwOpen_DoWork;
           
            pollSensorName = "CPU Package";
        }

        void bgwOpen_DoWork(object sender, DoWorkEventArgs e)
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
            if (!timer.IsEnabled && Working)
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

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateSensors();
            int cpuPackage = cpuSensors.FindIndex(sensor => sensor.Name == pollSensorName);
            if (cpuPackage == -1)
                cpuPackage = 0;
            
            temperature = cpuSensors[cpuPackage].Value;
            OnTempUpdated?.Invoke(this, new CpuTempEventArgs() { currentTemp= temperature});
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

                cpuSensors = hardware.Sensors.ToList<ISensor>();
                cpuSensors.RemoveAll(sens => sens.SensorType != SensorType.Temperature);
            }
            OnSensorUpdated?.Invoke(this, new EventArgs());
        }
    }

    public class CpuTempEventArgs : EventArgs
    {
        public float? currentTemp;
    }
}
