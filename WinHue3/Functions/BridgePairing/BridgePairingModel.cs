using WinHue3.Utils;

namespace WinHue3.Functions.BridgePairing
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
            get => _userMessage;
            set => SetProperty(ref _userMessage, value);
        }

        public double ProgressBarMax
        {
            get => _progressBarMax;
            set => SetProperty(ref _progressBarMax, value);
        }

        public double ProgressBarMin
        {
            get => _progressBarMin;
            set => SetProperty(ref _progressBarMin, value);
        }

        public double ProgressBarValue
        {
            get => _progressBarValue;
            set => SetProperty(ref _progressBarValue,value);
        }
    }
}
