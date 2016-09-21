using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    [DataContract,Serializable]
    public class SearchResult
    {
        public Dictionary<string, string> listnewobjects { get; set; }
        public string lastscan { get; set; }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }

    public class SearchResultJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof (SearchResult)) return null;
            SearchResult sr = new SearchResult {listnewobjects = new Dictionary<string, string>()};
            JObject obj = (JObject)serializer.Deserialize(reader);
            foreach (KeyValuePair<string,JToken> t in obj)
            {
                if (t.Key == "lastscan")
                    sr.lastscan = t.Value.ToString();
                else
                {
                    sr.listnewobjects.Add(t.Key,t.Value["name"].ToString());        
                }
            }         
            return sr;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SearchResult);
        }
    }
}