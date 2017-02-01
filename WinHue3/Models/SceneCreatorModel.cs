using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WinHue3.Resources;

namespace WinHue3.Models
{
    public class SceneCreatorModel : ValidatableBindableBase
    {

        private State _state;
        private ushort _hue;
        private byte _bri;
        private byte _sat;
        private decimal _x;
        private decimal _y;
        private int _tt;
        private bool _on;
        private string _name;

        public SceneCreatorModel()
        {
            State = new State() { @on = true };
            On = true;
            TT = -1;
        }

        public State State
        {
            get { return new State(); }
            set { SetProperty(ref _state, value); }
        }

        public ushort Hue
        {
            get { return _hue; }
            set { SetProperty(ref _hue, value); }
        }

        public byte Bri
        {
            get { return _bri; }
            set { SetProperty(ref _bri, value); }
        }

        public byte Sat
        {
            get { return _sat; }
            set { SetProperty(ref _sat, value); }
        }

        public decimal X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }

        public decimal Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        public int TT
        {
            get { return _tt; }
            set { SetProperty(ref _tt, value); OnPropertyChanged("TransitionTimeMessage"); }
        }

        public bool On
        {
            get { return _on; }
            set { SetProperty(ref _on, value); }
        }

        public string TransitionTimeMessage
        {
            get
            {
                if (TT >= 0)
                {
                    int time = (int)(TT * 100);
                    if (time == 0)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {GUI.MainForm_Sliders_TransitionTime_Instant}";
                    }
                    else if (time > 0 && time < 1000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {(double)time:0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Millisec}";
                    }
                    else if (time >= 1000 && time < 60000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double)time / 1000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Seconds}";
                    }
                    else
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double)time / 60000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Minutes}";
                    }
                }
                else
                {
                    return $"{GUI.MainForm_Sliders_TransitionTime} : {GUI.MainForm_Sliders_TransitionTime_Unit_None}";
                }
            }

        }


    }
}
