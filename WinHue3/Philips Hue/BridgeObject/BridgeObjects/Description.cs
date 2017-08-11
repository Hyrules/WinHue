using System.Xml.Serialization;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
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
            get => this.specVersionField;
            set => this.specVersionField = value;
        }

        /// <remarks/>
        public string URLBase
        {
            get => this.uRLBaseField;
            set => this.uRLBaseField = value;
        }

        /// <remarks/>
        public rootDevice device
        {
            get => this.deviceField;
            set => this.deviceField = value;
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
            get => this.majorField;
            set => this.majorField = value;
        }

        /// <remarks/>
        public byte minor
        {
            get => this.minorField;
            set => this.minorField = value;
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
            get => this.deviceTypeField;
            set => this.deviceTypeField = value;
        }

        /// <remarks/>
        public string friendlyName
        {
            get => this.friendlyNameField;
            set => this.friendlyNameField = value;
        }

        /// <remarks/>
        public string manufacturer
        {
            get => this.manufacturerField;
            set => this.manufacturerField = value;
        }

        /// <remarks/>
        public string manufacturerURL
        {
            get => this.manufacturerURLField;
            set => this.manufacturerURLField = value;
        }

        /// <remarks/>
        public string modelDescription
        {
            get => this.modelDescriptionField;
            set => this.modelDescriptionField = value;
        }

        /// <remarks/>
        public string modelName
        {
            get => this.modelNameField;
            set => this.modelNameField = value;
        }

        /// <remarks/>
        public string modelNumber
        {
            get => this.modelNumberField;
            set => this.modelNumberField = value;
        }

        /// <remarks/>
        public string modelURL
        {
            get => this.modelURLField;
            set => this.modelURLField = value;
        }

        /// <remarks/>
        public string serialNumber
        {
            get => this.serialNumberField;
            set => this.serialNumberField = value;
        }

        /// <remarks/>
        public string UDN
        {
            get => this.uDNField;
            set => this.uDNField = value;
        }

        /// <remarks/>
        public string presentationURL
        {
            get => this.presentationURLField;
            set => this.presentationURLField = value;
        }

        /// <remarks/>
        [XmlArrayItem("icon", IsNullable = false)]
        public rootDeviceIcon[] iconList
        {
            get => this.iconListField;
            set => this.iconListField = value;
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
            get => this.mimetypeField;
            set => this.mimetypeField = value;
        }

        /// <remarks/>
        public byte height
        {
            get => this.heightField;
            set => this.heightField = value;
        }

        /// <remarks/>
        public byte width
        {
            get => this.widthField;
            set => this.widthField = value;
        }

        /// <remarks/>
        public byte depth
        {
            get => this.depthField;
            set => this.depthField = value;
        }

        /// <remarks/>
        public string url
        {
            get => this.urlField;
            set => this.urlField = value;
        }
    }



}
