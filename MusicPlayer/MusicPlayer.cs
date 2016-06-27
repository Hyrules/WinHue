using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.Composition;
using WinHuePluginModule;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using HueLib_base;
using System.Threading;

namespace MusicPlayer
{
    [Export(typeof(IWinHuePluginModule))]
    class MusicPlayer : IWinHuePluginModule
    {
        [Import(typeof (IWinhuePluginHost))] 
        private IWinhuePluginHost Host;

        public bool? ShowSettingsForm()
        {
            SettingsForm sf = new SettingsForm();
            sf.Owner = Application.Current.MainWindow;
            sf.ShowDialog();
            return true;
        }

        private RECORDPROC _myRecProc;

        public void Start()
        {

            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            Bass.BASS_RecordInit(-1);
            _myRecProc = new RECORDPROC(MyRecording);
            int inputsource = 0;
            int settings = 0;
            float vol = 0;

            while (settings != -1)
            {
                settings = Bass.BASS_RecordGetInput(inputsource,ref vol);
                if (Bass.BASS_RecordGetInputName(inputsource) == "Speakers")
                {
                    break;
                }
                inputsource++;
            }

            Bass.BASS_RecordSetInput(inputsource, BASSInput.BASS_INPUT_ON, 0.5F);

            int recChannel = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, 50, _myRecProc, IntPtr.Zero);

            Bass.BASS_ChannelPlay(recChannel, false);

        }  

        private bool MyRecording(int handle, IntPtr buffer, int length, IntPtr user)
        {
            float[] fft = new float[length];
            
            Bass.BASS_ChannelGetData(handle, fft, length);
        //    foreach()
         //   int hz = Utils.FFTIndex2Frequency(1, length, 44100);
         //   Console.WriteLine(hz);
            return true;
        }

        public void Stop()
        {
           // throw new NotImplementedException();
        }

        public Bitmap pluginIcon => Properties.Resources.pluginicon;

        public string pluginName => "Music Player";

        public string pluginDesc => "Playing music is reflected in the lights.";

        public string pluginAuth => "Pascal Pharand";
    }
}
