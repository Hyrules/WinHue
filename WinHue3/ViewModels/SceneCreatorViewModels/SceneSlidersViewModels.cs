using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
namespace WinHue3.ViewModels
{
    public class SceneSlidersViewModels : ValidatableBindableBase
    {
        private State _state;
        private ushort _hue;
        private byte _bri;
        private byte _sat;
        private decimal _x;
        private decimal _y;


        public SceneSlidersViewModels()
        {
            State = new State() { @on = true };
        }

        public State State
        {
            get { return new State(); }
            set
            {
                SetProperty(ref _state, value);
            }
        }

        public ushort Hue
        {
            get { return _hue;}
            set { SetProperty(ref _hue, value); }
        }

        public byte Bri
        {
            get
            {
                return _bri;
            }

            set
            {
                SetProperty(ref _bri,value);
            }
        }

        public byte Sat
        {
            get
            {
                return _sat;
            }

            set
            {
                SetProperty(ref _sat,value);
            }
        }

        public decimal X
        {
            get
            {
                return _x;
            }

            set
            {
                SetProperty(ref _x,value);
            }
        }

        public decimal Y
        {
            get
            {
                return _y;
            }

            set
            {
                SetProperty(ref _y,value);
            }
        }
    }
}
