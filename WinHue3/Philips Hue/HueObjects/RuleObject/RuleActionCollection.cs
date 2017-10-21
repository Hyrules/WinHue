using System;
using System.Collections.Generic;
using System.ComponentModel;
using WinHue3.Annotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Action = WinHue3.Philips_Hue.HueObjects.RuleObject.RuleAction;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    public class RuleActionCollection : BindingList<RuleAction>, ICustomTypeDescriptor
    {

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < Count; i++)
            {
                pds.Add(new RuleActionItemDescriptor(this, i));
            }

            return pds;
            
        }

        public static implicit operator RuleActionCollection(List<RuleAction> lrac)
        {
            RuleActionCollection rac = new RuleActionCollection();
            foreach (var i in lrac)
            {
                rac.Add(i);
            }

            return rac;
        }

        public static implicit operator List<RuleAction>(RuleActionCollection rac)
        {
            List<RuleAction> lrac = new List<RuleAction>();
            foreach (var i in rac)
            {
                lrac.Add(i);   
            }
            return lrac;
        }

        public void AddRange(IEnumerable<RuleAction> collection)
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

    public class RuleActionItemDescriptor : PropertyDescriptor
    {
        private BindingList<RuleAction> _owner;
        private int _index;
        public RuleAction Value => _owner[_index];
        public static Attribute[] attrs = new Attribute[1] { new ExpandableObjectAttribute() };

        public RuleActionItemDescriptor(BindingList<RuleAction> owner, int index) : base($"Action {index}", attrs)
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
            _owner[_index] = (RuleAction)value;
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
