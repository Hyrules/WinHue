using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipOpenClose
{
    
    /// <summary>
    /// SensorState class.
    /// </summary>
    [DataContract]
    public class ClipOpenCloseSensorState : ValidatableBindableBase, ISensorState
    {
        private string _lastupdated;
        private bool _open;

        /// <summary>
        /// Open or close.
        /// </summary>
        [HueProperty, DataMember]
        public bool open
        {
            get => _open;
            set => SetProperty(ref _open,value);
        }

        [HueProperty, DataMember]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated,value);
        }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
