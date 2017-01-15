using System.Xml.Serialization;

namespace HueLib2
{

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    [XmlRoot(Namespace = "urn:schemas-upnp-org:device-1-0", IsNullable = false,ElementName="root")]

    public partial class Description
    {

        private rootSpecVersion specVersionField;

        private string uRLBaseField;

        private rootDevice deviceField;

        /// <remarks/>
        public rootSpecVersion specVersion
        {
            get
            {
                return this.specVersionField;
            }
            set
            {
                this.specVersionField = value;
            }
        }

        /// <remarks/>
        public string URLBase
        {
            get
            {
                return this.uRLBaseField;
            }
            set
            {
                this.uRLBaseField = value;
            }
        }

        /// <remarks/>
        public rootDevice device
        {
            get
            {
                return this.deviceField;
            }
            set
            {
                this.deviceField = value;
            }
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class rootSpecVersion
    {

        private byte majorField;

        private byte minorField;

        /// <remarks/>
        public byte major
        {
            get
            {
                return this.majorField;
            }
            set
            {
                this.majorField = value;
            }
        }

        /// <remarks/>
        public byte minor
        {
            get
            {
                return this.minorField;
            }
            set
            {
                this.minorField = value;
            }
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class rootDevice
    {

        private string deviceTypeField;

        private string friendlyNameField;

        private string manufacturerField;

        private string manufacturerURLField;

        private string modelDescriptionField;

        private string modelNameField;

        private string modelNumberField;

        private string modelURLField;

        private string serialNumberField;

        private string uDNField;

        private string presentationURLField;

        private rootDeviceIcon[] iconListField;

        /// <remarks/>
        public string deviceType
        {
            get
            {
                return this.deviceTypeField;
            }
            set
            {
                this.deviceTypeField = value;
            }
        }

        /// <remarks/>
        public string friendlyName
        {
            get
            {
                return this.friendlyNameField;
            }
            set
            {
                this.friendlyNameField = value;
            }
        }

        /// <remarks/>
        public string manufacturer
        {
            get
            {
                return this.manufacturerField;
            }
            set
            {
                this.manufacturerField = value;
            }
        }

        /// <remarks/>
        public string manufacturerURL
        {
            get
            {
                return this.manufacturerURLField;
            }
            set
            {
                this.manufacturerURLField = value;
            }
        }

        /// <remarks/>
        public string modelDescription
        {
            get
            {
                return this.modelDescriptionField;
            }
            set
            {
                this.modelDescriptionField = value;
            }
        }

        /// <remarks/>
        public string modelName
        {
            get
            {
                return this.modelNameField;
            }
            set
            {
                this.modelNameField = value;
            }
        }

        /// <remarks/>
        public string modelNumber
        {
            get
            {
                return this.modelNumberField;
            }
            set
            {
                this.modelNumberField = value;
            }
        }

        /// <remarks/>
        public string modelURL
        {
            get
            {
                return this.modelURLField;
            }
            set
            {
                this.modelURLField = value;
            }
        }

        /// <remarks/>
        public string serialNumber
        {
            get
            {
                return this.serialNumberField;
            }
            set
            {
                this.serialNumberField = value;
            }
        }

        /// <remarks/>
        public string UDN
        {
            get
            {
                return this.uDNField;
            }
            set
            {
                this.uDNField = value;
            }
        }

        /// <remarks/>
        public string presentationURL
        {
            get
            {
                return this.presentationURLField;
            }
            set
            {
                this.presentationURLField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("icon", IsNullable = false)]
        public rootDeviceIcon[] iconList
        {
            get
            {
                return this.iconListField;
            }
            set
            {
                this.iconListField = value;
            }
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class rootDeviceIcon
    {

        private string mimetypeField;

        private byte heightField;

        private byte widthField;

        private byte depthField;

        private string urlField;

        /// <remarks/>
        public string mimetype
        {
            get
            {
                return this.mimetypeField;
            }
            set
            {
                this.mimetypeField = value;
            }
        }

        /// <remarks/>
        public byte height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        public byte width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        public byte depth
        {
            get
            {
                return this.depthField;
            }
            set
            {
                this.depthField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }



}
