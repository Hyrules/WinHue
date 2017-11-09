using System;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Resources;
using WinHue3.ViewModels;

namespace WinHue3.Models
{
    public class SceneCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private bool _recycle;
        private State _state;

        public SceneCreatorModel()
        {
            State = new State() { on = true };
            On = true;
            TT = null;
            Recycle = false;
        }

        public State State
        {
            get => new State();
            set => SetProperty(ref _state, value);
        }

        public ushort? Hue
        {
            get => _state.hue;
            set
            {
                _state.hue = value;
                RaisePropertyChanged();
                if (value == null) return;
                Ct = null;
                X = null;
                Y = null;
            }
        }

        public byte? Bri
        {
            get => _state.bri;
            set { _state.bri = value; RaisePropertyChanged(); }
        }

        public byte? Sat
        {
            get => _state.sat;
            set { _state.sat = value; RaisePropertyChanged(); }
        }

        public decimal? X
        {
            get => _state.xy?[0];
            set
            {
                if (value != null)
                {
                    if (_state.xy == null) _state.xy = new decimal[2];
                    _state.xy[0] = Convert.ToDecimal(value);
                }
                if (Y < 0)
                    Y = 0;
                RaisePropertyChanged();
                RaisePropertyChanged("Y");
                if (value == null) return;
                Ct = null;
                Hue = null;
            }
        }

        public decimal? Y
        {
            get => _state.xy?[1];

            set
            {
                if (value != null)
                {
                    if (_state.xy == null) _state.xy = new decimal[2];
                    _state.xy[1] = Convert.ToDecimal(value);
                }
                if (X < 0)
                    X = 0;
                RaisePropertyChanged();
                RaisePropertyChanged("X");
                if (value == null) return;
                Ct = null;
                Hue = null;
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ushort? Ct
        {
            get => _state.ct;
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
            get => _state.transitiontime;
            set
            {
                _state.transitiontime = value; RaisePropertyChanged("TransitionTimeMessage"); }
        }

        public bool On
        {
            get => _state.on ?? true;
            set { _state.on = value; RaisePropertyChanged(); }
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

        public bool Recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }
    }
}
