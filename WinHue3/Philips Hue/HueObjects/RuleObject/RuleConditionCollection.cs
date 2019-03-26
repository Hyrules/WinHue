using System;
using System.Collections.Generic;
using System.ComponentModel;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects
{
    public class RuleConditionCollection : BindingList<RuleCondition>, ICustomTypeDescriptor
    {

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < Count; i++)
            {
                pds.Add(new RuleConditionItemDescriptor(this, i));
            }

            return pds;

        }

        public static implicit operator RuleConditionCollection(List<RuleCondition>lrac)
        {
            RuleConditionCollection rac = new RuleConditionCollection();
            foreach (var i in lrac)
            {
                rac.Add(i);
            }

            return rac;
        }

        public static implicit operator List<RuleCondition>(RuleConditionCollection rac)
        {
            List<RuleCondition> lrac = new List<RuleCondition>();
            foreach (var i in rac)
            {
                lrac.Add(i);
            }
            return lrac;
        }

        public void AddRange(IEnumerable<RuleCondition> collection)
        {
            foreach (var i in collection)
            {
                this.Items.Add(i);
            }
        }

        #region TYPE_DESCRIPTOR

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    public class RuleConditionItemDescriptor : PropertyDescriptor
    {
        private BindingList<RuleCondition> _owner;
        private int _index;
        public RuleCondition Value => _owner[_index];
        public static Attribute[] attrs = new Attribute[1] { new ExpandableObjectAttribute()} ;

        public RuleConditionItemDescriptor(BindingList<RuleCondition> owner, int index) : base($"Condition {index}", attrs )
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
            _owner[_index] = (RuleCondition)value;
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
