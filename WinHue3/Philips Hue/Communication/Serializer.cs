using System;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Philips_Hue.Communication
{
    /// <summary>
    /// Json Serializer / Deserializer
    /// </summary>
    public static class Serializer
    {

        private static readonly JsonSerializerSettings jss = new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default};

        public static object DeserializeToObject(string json, Type objecttype)
        {
            try
            {
                if (!json.Equals("{}") && !string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject(json, objecttype);
                }
                return null;
            }
            catch (Exception)
            {

                return null;
                
            }
           
        }

        /// <summary>
        /// This method serialize an object into a JSON string,
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON String</returns>
        public static string SerializeToJson(object obj)
        {
           
            try
            {
                if (obj != null)
                    return JsonConvert.SerializeObject(obj, jss);
                return null;
            }
            catch (Exception)
            {
                return null;
            }
      
        }

        /// <summary>
        /// This method deserialize a JSON string into an object
        /// </summary>
        /// <typeparam name="T">Type of object you want the string deserialize into</typeparam>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>The object result of the deserialized string</returns>


        public static T DeserializeToObject<T>(string json)
        {
            try
            {

                if (!json.Equals("{}") && !string.IsNullOrEmpty(json))
                {
                    return (T) JsonConvert.DeserializeObject<T>(json, jss);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }

        }

    }


}
