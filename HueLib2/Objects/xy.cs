using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    /// <summary>
    /// XY Class
    /// </summary>
    /// 
    [DataContract, JsonConverter(typeof(XYJsonConverter)),Serializable()]
    public class XY
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XY()
        {
            x = 0;
            y = 0;
        }
        /// <summary>
        /// X floating poing value.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false),Description("X Color space. (0-1.000)")]
        public decimal x { get; set; }

        /// <summary>
        /// Y floating point value.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false), Description("Y Color space. (0-1.000)")]
        public decimal y { get; set; }

        /// <summary>
        /// convert the XY value to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{x:0.000},{y:0.000}]";
        }

        /// <summary>
        /// Parse text to xy value
        /// </summary>
        /// <param name="text"></param>
        /// <returns>XY object created from the string.</returns>
        /// <exception cref="InvalidXYValueException">if text has not exactly an x and a y value.</exception>
        /// <exception cref="ArgumentNullException">if text is null.</exception>
        /// <exception cref="FormatException">if the x and y values are not of a float format.</exception>
        /// <exception cref="OverflowException">if the x and y value are over the max and the min values.</exception>
        public static XY Parse(string text)
        {
            XY result = new XY();
            Regex regex = new Regex(@"(0(\.\d+)?|1(\.0+)?)");
            MatchCollection mt = regex.Matches(text);
            if (mt.Count != 2) throw new InvalidXYValueException();
            result.x = decimal.Parse(mt[0].Groups[0].Value);
            result.y = decimal.Parse(mt[1].Groups[0].Value);
            return result;
        }

        /// <summary>
        /// Exception for invalid xy value while parsing.
        /// </summary>
        public class InvalidXYValueException : Exception {}
    }

    /// <summary>
    /// Convert the XY format to JSON and Back.
    /// </summary>
    public class XYJsonConverter : JsonConverter
    {
        /// <summary>
        /// If it is possible to Convert the type specified
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            if(objectType == typeof(XY))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Read the json
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="objectType">Object Type</param>
        /// <param name="existingValue">Existing Value</param>
        /// <param name="serializer">Serializer</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
          //  reader.Read();
            JArray obj = (JArray)serializer.Deserialize(reader);

            XY xy = new XY
            {
                x = Convert.ToDecimal(((JValue) obj[0]).Value),
                y = Convert.ToDecimal(((JValue) obj[1]).Value)
            };


            return xy;
        }

        /// <summary>
        /// Write Json
        /// </summary>
        /// <param name="writer">Json Writer</param>
        /// <param name="value">Value</param>
        /// <param name="serializer">Serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() != typeof(XY)) return;
            JArray o = new JArray();
            XY val = (XY)value;
            o.Add(val.x);
            o.Add(val.y);
            o.WriteTo(writer);
        }
      
    }
}
