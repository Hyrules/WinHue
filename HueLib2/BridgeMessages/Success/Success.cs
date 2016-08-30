using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    [DataContract, JsonConverter(typeof(SuccessJsonConverter))]
    public class Success : Message
    {
        [DataMember]
        public string Address
        {
            get{ return _address; }
            set
            {

                Regex reg;
                int nbrsl = value.Count(f => f == '/');
                reg = nbrsl == 3 ? new Regex("/(.*?)/(.*?)/(.*?) ") : new Regex("/(.*?)/(.*?)/(.*?)/(.*?) ");
                
                MatchCollection mc = reg.Matches(value + " ");
                if(mc.Count > 0)
                {
                    if (nbrsl == 4)
                    {
                        _obj = mc[0].Groups[1].Value.ToString();
                        _id = mc[0].Groups[2].Value.ToString();
                        _ds = mc[0].Groups[3].Value.ToString();
                        _prop = mc[0].Groups[4].Value.ToString();
                    }
                    else if( nbrsl == 3)
                    {
                        _obj = mc[0].Groups[1].Value.ToString();
                        _id = mc[0].Groups[2].Value.ToString();
                        _prop = mc[0].Groups[4].Value.ToString();
                    }
                    _address = value;
                }
            }
        }
        [DataMember]
        public string Value { get; set; }
        
        /// <summary>
        /// Whole Address of the success.
        /// </summary>
        private string _address;

        /// <summary>
        /// Object Light / Group / Sensor ect..
        /// </summary>
        private string _obj;

        /// <summary>
        /// Id of the object.
        /// </summary>
        private string _id;

        /// <summary>
        /// State or Action etc...
        /// </summary>
        private string _ds;

        /// <summary>
        /// Property eg : name
        /// </summary>
        private string _prop;

        public override string ToString()
        {
            return string.Format("address {0} to {1}",_address,Value);
        }

        public string id
        {
            get { return _id; }
            internal set { _id = value;}
        }

        public string obj
        {
            get { return _obj; }
            internal set { _obj = value;}
        }

        public string ds
        {
            get { return _ds; }
            set { _ds = value;}
        }

        public string property
        {
            get { return _prop; }
            internal set{ _prop = value;}
        }

    }

    public class SuccessJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Success) ? true : false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Success success = new Success();
            
            JObject obj = serializer.Deserialize<JObject>(reader);

            if (obj != null)
            {
                IList<string> keys = obj.Properties().Select(p => p.Name).ToList();
                JToken tok = (JToken)obj[keys[0]];
                string val;
                if (tok.Type == JTokenType.Array)
                {
                    val = ((JArray)tok).ToString();
                }
                else
                {
                    val = tok.ToString();
                }
                val = val.Replace("\r\n", string.Empty);
                success.Address = keys[0];
                success.Value = val;

            }

            return success;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
