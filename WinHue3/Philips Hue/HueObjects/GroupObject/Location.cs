using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    
    public sealed class Location : Dictionary<string, decimal[]>, ICustomTypeDescriptor
    {
        public override string ToString()
        {
            return "...";
        }

        #region TypeDescriptor       

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            foreach (string s in Keys)
            {
                pds.Add(new LocationPropertyDescriptor(this[s], s));
            }

            return pds;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    public class LocationPropertyDescriptor : PropertyDescriptor
    {
        private decimal[] _owner;
        private string _key;
        private decimal[] Value => _owner;

        public LocationPropertyDescriptor(decimal[] owner, string key) : base(key, null)
        {
            _owner = owner;
            _key = key;
        }

        public override string DisplayName => $"ID : {_key}";

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return $"X: {_owner[0]}, Y: {_owner[1]}, Z: {_owner[2]}";
        }

        public override void ResetValue(object component)
        {
            
        }

        public override void SetValue(object component, object value)
        {
            _owner = (decimal[]) value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => _owner.GetType();
        public override bool IsReadOnly => true;
        public override Type PropertyType => _owner.GetType();
    }
}