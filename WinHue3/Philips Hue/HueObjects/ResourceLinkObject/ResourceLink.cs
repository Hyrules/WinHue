using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Interface;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.ResourceLinkObject
{
    [JsonObject]
    public class Resourcelink : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _id;
        private string _description;
        private string _type;
        private ushort _classid;
        private string _owner;
        private bool? _recycle;
        private StringCollection _links;
        private bool _visible;

        /// <summary>
        /// ID of the ResourceLink
        /// </summary>
        [Category("Resource Link"),Description("ID of the resource link"), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }

        /// <summary>
        /// Image of the ResourceLink
        /// </summary>
        [Category("Resource Link"), Description("Image of the resource link"), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// Name of the resource link
        /// </summary>
        [Category("Resource Link"), Description("Name of the resource link")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// Description of the resource link
        /// </summary>
        [Category("Resource Link"),Description("Description of the resource link")]
        public string description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }

        /// <summary>
        /// Type of Resource Link
        /// </summary>
        [Category("Resource Link"),Description("Type of the resource link"), DontSerialize]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type,value);
        }

        /// <summary>
        /// Class of the resource link
        /// </summary>
        [Category("Resource Link"),Description("Class of the resource link"), CreateOnly]
        public ushort classid
        {
            get => _classid;
            set => SetProperty(ref _classid,value);
        }

        /// <summary>
        /// Owner of the resource link
        /// </summary>
        [Category("Resource Link"),Description("Owner of the resource link"), DontSerialize]
        public string owner
        {
            get => _owner;
            set => SetProperty(ref _owner,value);
        }

        /// <summary>
        /// Owner of the resource link
        /// </summary>
        [Category("Resource Link"),Description("Allow Recycle of the resource link"),CreateOnly]
        public bool? recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }

        /// <summary>
        /// List of resource links
        /// </summary>
        [Category("Resource Link"),Description("List of resource links")]
        public StringCollection links
        {
            get => _links;
            set => SetProperty(ref _links,value);
        }

        [ReadOnly(true), JsonIgnore, Browsable(false)]
        public bool visible
        {
            get { return _visible; }
            set { SetProperty(ref _visible,value); }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        [OnDeserialized]
        void OnDeserialize(StreamingContext ctx)
        {
            Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
        }

        public override bool Equals(object obj)
        {
            return obj is Resourcelink hueobject && hueobject.Id == Id;
        }

        public override string ToString()
        {
            return name;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
