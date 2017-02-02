using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Common properties for Action and State
    /// </summary>
    [DataContract]
    public class CommonProperties : RuleBody
    {

        private ushort? _ct;

        /// <summary>
        /// On state of the group.
        /// </summary>
        [DataMember(IsRequired = false), Description("On state."), Category("Properties")]
        public bool? on { get; set; }
        /// <summary>
        /// Brightness of the group.
        /// </summary>
        [DataMember(IsRequired = false), Description("Brightness(0-254)"), Category("Properties")]
        public byte? bri { get; set; }
        /// <summary>
        /// Hue/Color of the group.
        /// </summary>
        [DataMember(IsRequired = false), Description("Color (0-65535)"), Category("Properties")]
        public ushort? hue { get; set; }
        /// <summary>
        /// Saturation of the group.
        /// </summary>
        [DataMember(IsRequired = false), Description("Saturation (0-254)"), Category("Properties")]
        public byte? sat { get; set; }
        /// <summary>
        /// Float color of the group.
        /// </summary>
        [DataMember(IsRequired = false), ItemsSource(typeof(XY)),ExpandableObject, Description("Color coordinates in CIE color space. ( float value between 0.000 and 1.000)"), Category("Properties")]
        public XY xy { get; set; }

        /// <summary>
        /// Color temperature of the group.
        /// </summary>
        [DataMember(IsRequired = false), MinValue(153), MaxValue(500),
        Description("Color temperature in Mired. (Value from 153[6500K] to 500[2000K])"), Category("Properties")]
        public ushort? ct
        {
            get
            {
                if (_ct == null) return null;
                MemberInfo[] meminfo = this.GetType().GetMember("ct");
                if (meminfo.Length <= 0) return null;
                MaxValue max = (MaxValue)meminfo[0].GetCustomAttribute(typeof(MaxValue));
                MinValue min = (MinValue) meminfo[0].GetCustomAttribute(typeof(MinValue));
                if(((ushort)_ct >= min.Min) && ((ushort)_ct <= max.Max)) return _ct;
                else if ((ushort)_ct < min.Min) return min.Min;
                else if ((ushort) _ct > max.Max) return max.Max;
                return null;
            }
            set { _ct = value; }
        }
        /// <summary>
        /// effect of the group.
        /// </summary>
        [DataMember(IsRequired = false), ItemsSource(typeof(EffectItemsSource)), Description("Dynamic effect. ( none or colorloop )"), Category("Properties")]
        public string effect { get; set; }
        /// <summary>
        /// Color mode of the group.
        /// </summary>
        [DataMember(IsRequired = false), ReadOnly(true), Description("Color mode. ( hs for hue saturation, xy for XY , ct for color temperature )"), Category("Properties")]
        public string colormode { get; set; }
        /// <summary>
        /// Transition time of the group.
        /// </summary>
        [DataMember(IsRequired = false), Description("Transition time ( Given in multiple of 100ms , Default to 4 )"), Category("Properties"), Browsable(false)]
        public uint? transitiontime { get; set; }
        /// <summary>
        /// Alert of the group.
        /// </summary>
        [DataMember(IsRequired = false), ItemsSource(typeof(AlertItemsSource)), Description("Alert Effect ( none , select or lselect )"), Category("Properties")]
        public string alert { get; set; }

        /// <summary>
        /// Brightness increment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Brightness increment."), Category("Incrementors")]
        public short? bri_inc { get; set; }
        /// <summary>
        /// Saturation increment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Saturation increment."), Category("Incrementors")]
        public short? sat_inc { get; set; }

        /// <summary>
        /// Hue increment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Saturation increment."), Category("Incrementors")]
        public int? hue_inc { get; set; }

        /// <summary>
        /// Color temperature increment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Color temperature increment."), Category("Incrementors")]
        public int? ct_inc { get; set; }

        /// <summary>
        /// XY increment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("XY increment."), Category("Incrementors")]
        public float? xy_inc { get; set; }

        /// <summary>
        /// Merge 2 object which properties are not null. If the property is not null in the target it will not copy and will be ignored.
        /// </summary>
        /// <param name="target">Target properties</param>
        /// <param name="source">Source properties to copy from</param>
        /// <returns>The merge values of both properties.</returns>
        public static CommonProperties operator +(CommonProperties target, CommonProperties source)
        {

            Type t = typeof(CommonProperties);

            PropertyInfo[] propertyInfos = t.GetProperties();

            foreach (PropertyInfo prop in propertyInfos)
            {
                if(prop.GetValue(target) == null && prop.GetValue(source) != null)
                    prop.SetValue(target,prop.GetValue(source));    

            }
            
            return target;
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

    }

    /// <summary>
    /// Maximum value attribute
    /// </summary>
    public class MaxValue : Attribute
    {
        /// <summary>
        /// Maximum value
        /// </summary>
        public ushort Max;

        /// <summary>
        /// Set Max value
        /// </summary>
        /// <param name="max"></param>
        public MaxValue(ushort max)
        {
            Max = max;
        }
    }

    /// <summary>
    /// Minimum value attribute
    /// </summary>
    public class MinValue : Attribute
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public ushort Min;

        /// <summary>
        /// Set Minimum value
        /// </summary>
        /// <param name="min"></param>
        public MinValue(ushort min)
        {
            Min = min;
        }

    }
}
