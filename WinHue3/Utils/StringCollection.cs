using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Utils
{
    [Serializable,ExpandableObject]
    public sealed class StringCollection : BindingList<string>, ICustomTypeDescriptor
    {
        public StringCollection() : base(new BindingList<string>())
        {
            
        }

        public static implicit operator StringCollection(List<string> list)
        {          
            return new StringCollection(list);
        }

        public static implicit operator List<string>(StringCollection sc)
        {
            List<string> list = new List<string>();

            foreach (var i in sc)
            {
                list.Add(i);
            }

            return list;
        }

        public StringCollection(List<string> list)
        {
            foreach (var i in list)
            {
                this.Items.Add(i);
            }
            
        }

        public void AddRange(IEnumerable<string> collection)
        {
            foreach (var i in collection)
            {
                this.Items.Add(i);
            }
        }

        public override string ToString()
        {
            return string.Join(",", this.Items);
        }


        #region TYPE_DESCRIPTOR

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < Count; i++)
            {
                pds.Add(new StringPropertyDescriptor(this, i));
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
            return TypeDescriptor.GetEditor(this,editorBaseType,true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes,true);
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

    public class StringPropertyDescriptor : PropertyDescriptor
    {
        private BindingList<string> _owner;
        private int _index;
        private string Value => _owner[_index];

        public StringPropertyDescriptor(BindingList<string> owner, int index) : base(index.ToString(), null)
        {
            _owner = owner;
            _index = index;
        }

        

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return Value;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            _owner[_index] = value.ToString();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => _owner.GetType();
        public override bool IsReadOnly => true;
        public override Type PropertyType => Value.GetType();
    }
}
