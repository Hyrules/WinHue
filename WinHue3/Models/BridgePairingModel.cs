using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3.Models
{
    public class BridgePairingModel : ValidatableBindableBase
    {

        private double _progressBarMax;
        private double _progressBarMin;
        private string _userMessage;
        private double _progressBarValue;

        public BridgePairingModel()
        {
            _progressBarMax = 60;
            _progressBarMin = 0;
            _progressBarValue = 0;
            _userMessage = GlobalStrings.BridgeDetectionPairing_SelectionMessage;
        }

        public string UserMessage
        {
            get { return _userMessage; }
            set { SetProperty(ref _userMessage, value); }
        }

        public double ProgressBarMax
        {
            get { return _progressBarMax; }
            set { SetProperty(ref _progressBarMax, value); }
        }

        public double ProgressBarMin
        {
            get { return _progressBarMin; }
            set { SetProperty(ref _progressBarMin, value); }
        }

        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set { SetProperty(ref _progressBarValue,value); }
        }
    }
}
