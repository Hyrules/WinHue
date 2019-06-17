using System;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Resources;
using WinHue3.Utils;

namespace WinHue3.Functions.Scenes.Creator
{
    public class SceneCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private bool _recycle;
        private State _state;
        private string _type;

        public SceneCreatorModel()
        {
            _name = string.Empty;
            _state = new State() { @on = true };
            _state.on = true;
            _state.transitiontime = null;
            _recycle = false;
            _type = "LightScene";
        }

        public State State
        {
            get => new State();
            set => SetProperty(ref _state, value);
        }

        public ushort? Hue
        {
            get => _state.hue;
            set { _state.hue = value;RaisePropertyChanged(); }
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
                if (value == null)
                    _state.xy = null;
                else
                {
                    if(_state.xy == null)
                        _state.xy = new decimal[2];
                    _state.xy[0] = Convert.ToDecimal(value);
                    RaisePropertyChanged();
                }
            }
        }

        public decimal? Y
        {
            get => _state.xy?[1];
            set
            {
                if (value == null)
                    _state.xy = null;
                else
                {
                    if (_state.xy == null)
                        _state.xy = new decimal[2];
                    _state.xy[1] = Convert.ToDecimal(value);
                    RaisePropertyChanged();
                }
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
            set { _state.ct = value; RaisePropertyChanged();}
        }

        public ushort? TT
        {
            get => _state.transitiontime;
            set { _state.transitiontime = value; RaisePropertyChanged(); }
        }

        public bool On
        {
            get => _state.on ?? true;
            set { _state.on = value; RaisePropertyChanged(); }
        }


        public bool Recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type,value);
        }
    }
}
