using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib
{
    /// <summary>
    /// Error class
    /// </summary>
    [DataContract,JsonConverter(typeof(ErrorMessageConverter))]
    public class Error : Message
    {
        /// <summary>
        /// Type of error.
        /// </summary>
        [DataMember]
        public int type { get; set; }
        /// <summary>
        /// Address of the error.
        /// </summary>
        [DataMember(EmitDefaultValue=false,IsRequired=false)]
        public string address { get; set; }
        /// <summary>
        /// Description of the error.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        public override string ToString()
        {
            return string.Format("Type : {0}, {1} at address {2}.",type,description,address);
        }

  
    }

    public class ErrorMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Error) ? true : false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            Error err = new Error();
            foreach(var o in obj)
            {
                switch(o.Key)
                {
                    case "type":
                        err.type = (int)o.Value;
                        break;
                    case "address":
                        err.address = o.Value.ToString();
                        break;
                    case "description":
                        err.description = o.Value.ToString();
                        break;
                }
            }

            return err;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
        
}
