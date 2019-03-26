using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WinHue3.Philips_Hue.Communication
{
    /// <summary>
    /// Json Serializer / Deserializer
    /// </summary>
    public static class Serializer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
        private static readonly HueCreateDataContractResolver hcr = new HueCreateDataContractResolver();
        private static readonly HueModifyDataContractResolver hmc = new HueModifyDataContractResolver();
        private static readonly DefaultContractResolver dcr = new DefaultContractResolver();

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

        public static string SerializeJsonObject(object obj)
        {
            jss.ContractResolver = dcr;
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
        /// This method serialize an object into a JSON string,
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON String</returns>
        public static string CreateJsonObject(object obj)
        {
            jss.ContractResolver = hcr;
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
        /// The method serialze an object into a JSON string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ModifyJsonObject(object obj)
        {
            jss.ContractResolver = hmc;
            try
            {
                if (obj != null)
                    return JsonConvert.SerializeObject(obj, jss);
                return null;
            }
            catch(Exception)
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
                    return (T)JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});
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
