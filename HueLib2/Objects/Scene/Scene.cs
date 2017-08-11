using System;
using System.Collections.Generic;
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
    /// Scene Class.
    /// </summary>
    [DataContract, Serializable, HueType("scenes")]
    public class Scene : IHueObject
    {
        private string _name;
        private ImageSource _image;

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Scene Properties"), Description("Image of the Scene"),  ReadOnly(true), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Scene Properties"), Description("ID of the Scene"), ReadOnly(true), Browsable(false)]
        public string Id { get; set; }

        /// <summary>
        /// Name of the scene.
        /// </summary>
        [DataMember(Name="name"), Category("Scene Properties"), Description("Name of the scene")]
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
        /// List of the light in the scene. 
        /// </summary>
        [DataMember, Category("Scene Properties"), Description("Lights of the scene"),Browsable(false)]
        public List<string> lights {get;set;}

        /// <summary>
        /// Owner of the scene.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Whitelist user that created or modified the content of the scene"), ReadOnly(true)]
        public string owner { get; set; }

        /// <summary>
        /// App specific data.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("App specific data linked to the scene."),ExpandableObject, ReadOnly(true)]
        public AppData appdata { get; set; }

        /// <summary>
        /// Scene can be deleted by the bridge automatically.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Indicates whether the scene can be automatically deleted by the bridge."), CreateOnly]
        public bool? recycle { get; set; }

        /// <summary>
        /// Scene is Locked.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Indicates that the scene is locked by a rule or a schedule and cannot be deleted until all resources requiring or that reference the scene are deleted."), ReadOnly(true)]
        public bool? locked { get; set; }

        /// <summary>
        /// Picture path
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Path to the picture."), CreateOnly]
        public string picture { get; set; }

        /// <summary>
        /// Version of the scene.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Version of scene document."), ReadOnly(true)]
        public int? version { get; set; }

        /// <summary>
        /// Last time the scene was updated in UTC.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("UTC time the scene has been created or has been updated."), ReadOnly(true)]
        public string lastupdated { get; set; }

        /// <summary>
        /// State of the lights in the scene.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("States of every lights in the scene."),Browsable(false), ReadOnly(true)]
        public Dictionary<string, State> lightstates { get; set; }

        /// <summary>
        /// Store current light state in scene.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Store the current light state in the scene."),Browsable(false)]
        public bool? storelightstate { get; set; }

        /// <summary>
        /// Transition time of the scene.
        /// </summary>
        [DataMember(IsRequired = false), Category("Scene Properties"), Description("Transition time of the scene."), Browsable(true)]
        public ushort? transitiontime { get; set; }

        /// <summary>
        /// To String.
        /// </summary>
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
