using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Utils
{
    [Serializable, ExpandableObject]
    public class CoordinatesCollection : BindingList<Tuple<float, float>>, ICustomTypeDescriptor
    {

        public CoordinatesCollection() : base(new BindingList<Tuple<float, float>>())
        {

        }

        public CoordinatesCollection(List<Tuple<float, float>> list)
        {
            foreach (var i in list)
            {
                this.Items.Add(i);
            }
        }



        public static implicit operator CoordinatesCollection(List<Tuple<float, float>> list)
        {
            return new CoordinatesCollection(list);
        }

        public void AddRange(IEnumerable<Tuple<float, float>> collection)
        {
            foreach (var i in collection)
            {
                this.Items.Add(i);
            }
        }

        #region TYPE_DESCRIPTOR

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < Count; i++)
            {
                pds.Add(new CoordinatesPropertyDescriptor(this, i));
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
    public class CoordinatesPropertyDescriptor : PropertyDescriptor
    {
        private BindingList<Tuple<float,float>> _owner;
        private int _index;
        private Tuple<float,float> Value => _owner[_index];

        public CoordinatesPropertyDescriptor(BindingList<Tuple<float,float>> owner, int index) : base(index.ToString(), null)
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
            _owner[_index] = (Tuple<float,float>)value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => _owner.GetType();
        public override bool IsReadOnly => false;
        public override Type PropertyType => Value.GetType();
    }
}
