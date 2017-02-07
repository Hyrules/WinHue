using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WinHue3.Resources;

namespace WinHue3.Models
{
    public class SceneCreatorModel : ValidatableBindableBase
    {
        private string _name;

        private State _state;

        public SceneCreatorModel()
        {
            State = new State() { on = true };
            On = true;
            TT = null;
        }

        public State State
        {
            get { return new State(); }
            set { SetProperty(ref _state, value); }
        }

        public ushort? Hue
        {
            get { return _state.hue; }
            set
            {
                _state.hue = value;
                OnPropertyChanged();
                if (value == null) return;
                Ct = null;
                X = null;
                Y = null;
            }
        }

        public byte? Bri
        {
            get { return _state.bri; }
            set { _state.bri = value; OnPropertyChanged(); }
        }

        public byte? Sat
        {
            get { return _state.sat; }
            set { _state.sat = value; OnPropertyChanged(); }
        }

        public decimal? X
        {
            get { return _state.xy?.x; }
            set
            {
                if (value != null)
                {
                    if (_state.xy == null) _state.xy = new XY();
                    _state.xy.x = Convert.ToDecimal(value);
                }
                OnPropertyChanged();
                OnPropertyChanged("Y");
                if (value == null) return;
                Ct = null;
                Hue = null;
            }
        }

        public decimal? Y
        {
            get { return _state.xy?.y; }
            set
            {
                if (value != null)
                {
                    if (_state.xy == null) _state.xy = new XY();
                    _state.xy.y = Convert.ToDecimal(value);
                }

                OnPropertyChanged();
                OnPropertyChanged("X");
                if (value == null) return;
                Ct = null;
                Hue = null;
            }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public ushort? Ct
        {
            get { return _state.ct; }
            set
            {
                _state.ct = value;
                if (value == null) return;
                X = null;
                Y = null;
                Hue = null;
            }
        }

        public uint? TT
        {
            get { return _state.transitiontime; }
            set
            {
                _state.transitiontime = value; OnPropertyChanged("TransitionTimeMessage"); }
        }

        public bool On
        {
            get { return _state.on ?? true; }
            set { _state.on = value; OnPropertyChanged(); }
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
