using System;
using Newtonsoft.Json;

namespace WinHue3.Philips_Hue.Communication
{
    /// <summary>
    /// Json Serializer / Deserializer
    /// </summary>
    public static class Serializer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly JsonSerializerSettings jss = new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default};


        public static object DeserializeToObject(string json, Type objecttype)
        {
            log.Debug(json);
            try
            {
                if (!json.Equals("{}") && !string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject(json, objecttype);
                }
                else
                {
                    log.Warn("Received string was empty.");
                }
                return null;
            }
            catch (Exception)
            {
                
                log.Error($"Error deserializing object {objecttype.Name} : " + json);
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
                log.Error($"Error serializing object {obj }");
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
            log.Debug(json);
            try
            {

                if (!json.Equals("{}") && !string.IsNullOrEmpty(json))
                {
                    return (T) JsonConvert.DeserializeObject<T>(json, jss);
                }
                else
                {
                    log.Warn("Received string was empty.");
                }

                return default(T);
            }
            catch (Exception)
            {
                log.Error($"Error deserializing object {typeof(T).Name} : " + json);
                return default(T);
            }

        }

    }


}
