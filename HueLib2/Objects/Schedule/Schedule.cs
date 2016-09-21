using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Class for a schedule.
    /// </summary>
    [DataContract, DefaultProperty("Schedule"),Serializable]
    public class Schedule : HueObject
    {
        private string _name;
        /// <summary>
        /// Name of the Schedule.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Name of the schedule"), HueLib(true, true)]
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        //      [DataMember, Category("Schedule Properties"), Description("UTC Time of the schedule")]
        //     public string time { get; set; }
        /// <summary>
        /// Time when the scheduled event will occur in ISO 8601:2004 format.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Local Time of the schedule"), HueLib(true, true)]
        public string localtime { get; set; }
        /// <summary>
        /// Description of the schedule
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Description of the schedule"), HueLib(true, true)]
        public string description { get; set; }

        /// <summary>
        /// Description of the schedule
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Time of the schedule"),Obsolete("Please use local time instead of time."), HueLib(true, true)]
        public string time { get; set; }

        /// <summary>
        /// Command to be executed when the schedule is triggered
        /// </summary>
        [DataMember, ExpandableObject, Category("Command"), Description("Command of the schedule"), HueLib(true, true)]
        public Command command { get; set; }

        /// <summary>
        /// Status of the schedule.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule"), HueLib(true, true)]
        public string status { get; set; }

        /// <summary>
        /// Recycle the schedule.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule"), HueLib(true, false)]
        public bool? recycle { get; set; }

        /// <summary>
        /// Date created.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule"), HueLib(false, false)]
        public string created { get; set; }

        /// <summary>
        /// Autodelete.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Autodelete the schedule"), HueLib(true, true)]
        public bool? autodelete { get; set; }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }

}
