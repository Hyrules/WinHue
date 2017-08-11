using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Markup;
using System.Windows.Media;
using HueLib2.Objects.Group;
using HueLib2.Objects.HueObject;
using HueLib2.Objects.Interfaces;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Group Class.
    /// </summary>
    [DataContract, DefaultProperty("Group"), HueType("groups")]
    public class Group : IHueObject
    {
        private string _name;
        private ImageSource _image;

        /// <summary>
        /// Image of the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Image of the group"), ReadOnly(true), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("ID of the group"), ReadOnly(true)]
        public string Id { get; set; }

        /// <summary>
        /// Action (State) of the group
        /// </summary>
        [DataMember, Category("Action"), Description("Action"),ExpandableObject, ReadOnly(true)]
        public Action action { get; set; }

        [DataMember, Category("State"), Description("State"),ExpandableObject, ReadOnly(true)]
        public GroupState state { get; set; }
        /// <summary>
        /// List of lights in the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Lights in the group"),Browsable(false)]
        public List<string> lights { get; set;}
        /// <summary>
        /// Group name.
        /// </summary>
        [DataMember(Name = "name"), Category("Group Properties"), Description("Name of the group")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Type of the group
        /// </summary>
        [DataMember, Category("Group Properties"), Description("The type of group"), CreateOnly]
        public string type { get; set; }

        /// <summary>
        /// Model ID
        /// </summary>
        [JsonIgnore]
        [DataMember, Category("Group Properties"), Description("Model id of the group")]
        public string modelid { get; set; }

        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonIgnore]
        [DataMember, Category("Group Properties"), Description("Unique id of group")]
        public string uniqueid { get; set; }

        /// <summary>
        /// Class
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Class of the group")]
        public string @class { get; set; }

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
