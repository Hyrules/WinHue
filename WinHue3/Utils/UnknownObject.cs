using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Annotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Utils
{
    [JsonConverter(typeof(HueObjectSerializer)), ExpandableObject, TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class HueObject : DynamicObject, ICustomTypeDescriptor, INotifyPropertyChanged, IEnumerable<KeyValuePair<string, object>>
    {
        private IDictionary<string, object> properties = new Dictionary<string, object>();

        public HueObject(string key, object value)
        {
            properties.Add(key, value);
        }

        public HueObject()
        {

        }

        public HueObject(IDictionary<string, object> dic)
        {
            properties = dic;
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var o in properties)
            {
                yield return o.Key;
            }
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {

            if (properties.ContainsKey(binder.Name))
            {
                result = properties[binder.Name];
                return true;
            }

            result = null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (properties.ContainsKey(binder.Name))
            {
                properties[binder.Name] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));
            }
            else
            {
                if (value != null)
                {
                    properties.Add(binder.Name, value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));
                }
            }
            return true;
        }

        public void SetProperty([NotNull] string property, object value)
        {
            if (properties.ContainsKey(property))
            {
                properties[property] = value;

            }
            else
            {
                if (value != null)
                    properties.Add(property, value);
            }
        }

        public void RemoveProperty([NotNull] string key)
        {
            try
            {
                properties.Remove(key);
            }
            catch (KeyNotFoundException)
            {

            }
        }

        public object this[string name]
        {
            get => properties[name];
            set => properties[name] = value;
        }

        public static implicit operator Dictionary<string, object>(HueObject hobj)
        {
            return (Dictionary<string, object>)hobj.properties;
        }

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
            return null;

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
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            Attribute[] attributes = new Attribute[0];
            var props = this.properties.Select(pair =>
                new HueObjectPropertyDescriptor(this, pair.Value.GetType(), pair.Key, attributes));

            return new PropertyDescriptorCollection(props.ToArray<PropertyDescriptor>());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var props = this.properties.Select(pair =>
                new HueObjectPropertyDescriptor(this, pair.Value.GetType(), pair.Key, attributes));
            return new PropertyDescriptorCollection(props.ToArray<PropertyDescriptor>());

        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        private class HueObjectPropertyDescriptor : PropertyDescriptor
        {
            private readonly HueObject _hueObject;
            private readonly Type _propertyType;


            public HueObjectPropertyDescriptor([NotNull] HueObject hobj, Type propertyType, [NotNull] string name, Attribute[] attrs) : base(name, attrs)
            {
                _hueObject = hobj;
                _propertyType = propertyType;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                return _hueObject.properties[Name];
            }

            public override void ResetValue(object component)
            {

            }

            public override void SetValue(object component, object value)
            {
                _hueObject.properties[Name] = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override Type ComponentType => typeof(HueObject);
            public override bool IsReadOnly => false;
            public override Type PropertyType => _propertyType;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class HueObjectSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            HueObject hue = (HueObject)value;

            SerializeHueObject(hue, writer);
        }

        private void SerializeHueObject(HueObject obj, JsonWriter writer)
        {
            obj.RemoveProperty("Id");
            obj.RemoveProperty("Image");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, object> kvp in obj)
            {
                writer.WritePropertyName(kvp.Key);
                if (kvp.Value is HueObject)
                {
                    SerializeHueObject((HueObject)kvp.Value, writer);
                }
                else
                {
                    if (kvp.Value is Array || kvp.Value.GetType() == typeof(List<>))
                    {
                        writer.WriteStartArray();
                        foreach (object o in (Array)kvp.Value)
                        {
                            if (o is HueObject)
                            {
                                SerializeHueObject((HueObject)o, writer);
                            }
                            else
                            {
                                writer.WriteValue(o);
                            }

                        }
                        writer.WriteEnd();
                    }
                    else
                    {
                        writer.WriteValue(kvp.Value);
                    }

                }
            }
            writer.WriteEnd();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            return ParseObject(obj);
        }

        private object ParseObject(JToken token)
        {

            switch (token.Type)
            {
                case JTokenType.Object:
                    HueObject hobj = new HueObject();
                    foreach (var child in token.Children<JProperty>())
                    {
                        hobj.SetProperty(child.Name, ParseObject(child.Value));
                    }
                    return hobj;
                case JTokenType.Array:
                    return token.Select(ParseObject).ToArray();
                //return token.Select(ParseObject).ToList();
                default:
                    return ((JValue)token).Value;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HueObject);
        }

    }
}