using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericStatusSensorState  : ValidatableBindableBase, ISensorStateBase
    {
        private int _status;


        /// <summary>
        /// Sensor Status.
        /// </summary>
        [DataMember,JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int status
        {
            get => _status;
            set =>  SetProperty(ref _status,value);
        }

        private string _lastupdated;

        [DataMember, ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
