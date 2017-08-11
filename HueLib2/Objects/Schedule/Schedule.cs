using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Media;
using HueLib2.Objects.HueObject;
using HueLib2.Objects.Interfaces;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Class for a schedule.
    /// </summary>
    [DataContract, DefaultProperty("Schedule"),Serializable, HueType("schedules")]
    public class Schedule : IHueObject
    {
        private string _name;
        private ImageSource _image;

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Image of the Schedule"), ReadOnly(true), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("ID of the Schedule"), ReadOnly(true), Browsable(false)]
        public string Id { get; set; }

        /// <summary>
        /// Name of the Schedule.
        /// </summary>
        [DataMember(Name = "name"), Category("Schedule Properties"), Description("Name of the schedule")]
        public string Name
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
        [DataMember, Category("Schedule Properties"), Description("Local Time of the schedule")]
        public string localtime { get; set; }
        /// <summary>
        /// Description of the schedule
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Description of the schedule")]
        public string description { get; set; }

        /// <summary>
        /// Description of the schedule
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Time of the schedule"),Obsolete("Please use local time instead of time.")]
        public string time { get; set; }

        /// <summary>
        /// Command to be executed when the schedule is triggered
        /// </summary>
        [DataMember, ExpandableObject, Category("Command"), Description("Command of the schedule")]
        public Command command { get; set; }

        /// <summary>
        /// Status of the schedule.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule")]
        public string status { get; set; }

        /// <summary>
        /// Recycle the schedule.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule"), CreateOnly]
        public bool? recycle { get; set; }

        /// <summary>
        /// Date created.
        /// </summary>
        [DataMember, ExpandableObject, Category("Schedule Properties"), Description("Command of the schedule")]
        public string created { get; set; }

        /// <summary>
        /// Autodelete.
        /// </summary>
        [DataMember, Category("Schedule Properties"), Description("Autodelete the schedule")]
        public bool? autodelete { get; set; }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        /// <summary>
        /// Event that happen when property has change.
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When a property has change this event is triggered - needed for the binding to refresh properly.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
