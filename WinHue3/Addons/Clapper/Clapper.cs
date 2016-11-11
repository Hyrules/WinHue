using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NAudio.Wave;

namespace WinHue3
{
    public class Clapper
    {
        private float bigValue;
        WaveIn waveIn;
        private double MaxValue;
        private int clapcount = 0;
        private bool isrunning = false;
        DispatcherTimer dt = new DispatcherTimer();
        public Clapper()
        {
            dt.Interval = new TimeSpan(0,0,0,0,2000);
            dt.Tick += Dt_Tick;
           
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            clapcount = 0;
            Console.WriteLine("Rest timer...");
        }

        public void Start()
        {

            MaxValue = (double)60 / 100;
            bigValue = 0;
            waveIn = new WaveIn();
            int waveInDevices = WaveIn.DeviceCount;
            //get the devicecount

            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
            }
            //load in the deviceinfo using the GetCapabilties function

            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
            int sampleRate = 8000; // 8 kHz
            int channels = 1; // mono
            waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            //Setting the format
            waveIn.StartRecording();
            dt.Start();
            isrunning = true;
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            float sample32 = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                sample32 = sample / 32768f;

                if (bigValue < sample32)
                {
                    bigValue = sample32;
                    if (bigValue > MaxValue)
                    {
                        MessageBox.Show("Clap !");

                    }
                }
            }





        }

        
       
    }
}
