using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HueLib2.Objects.HueObject;
using HueLib2.Objects.Interfaces;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    [DefaultProperty("Resource Links"), DataContract, HueType("resourcelinks")]
    public class Resourcelink : IHueObject
    {
        private string _name;
        private ImageSource _image;

        /// <summary>
        /// ID of the ResourceLink
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("ID of the resource link")]
        public string Id { get; set; }

        /// <summary>
        /// Image of the ResourceLink
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Image of the resource link"), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set {_image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Name of the resource link
        /// </summary>
        [DataMember(Name = "name",EmitDefaultValue = false, IsRequired = false), Category("Resource Link"),Description("Name of the resource link")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Description of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Description of the resource link")]
        public string description { get; set; }

        /// <summary>
        /// Type of Resource Link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Type of the resource link"), ReadOnly(true)]
        public string type { get; set; }
        
        /// <summary>
        /// Class of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Class of the resource link"),CreateOnly]
        public ushort classid { get; set; }

        /// <summary>
        /// Owner of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Owner of the resource link"), ReadOnly(true)]
        public string owner { get; set; }

        /// <summary>
        /// Owner of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Allow Recycle of the resource link"), CreateOnly]
        public bool? recycle { get; set; }

        /// <summary>
        /// List of resource links
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("List of resource links"), Browsable(false)]
        public List<string> links { get; set; }

        /// <summary>
        /// Event that happen when property has change.
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }
}
