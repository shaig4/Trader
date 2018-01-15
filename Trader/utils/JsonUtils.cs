using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Trader
{
    /// <summary>
    /// common functions for handling xmls
    /// </summary>
    public  class JsonUtils
    {
        public static string Serialize(object data, bool serializeNullValues = true)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                NullValueHandling = serializeNullValues ? NullValueHandling.Include : NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            return JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, jsonSetting);
        }

        public static T Decode<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static Dictionary<string, object> DynamicToDict(object dynObj)
        {
            var json = Serialize(dynObj);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public static JObject Parse(string v)
        {
            return JObject.Parse(v);
        }

        public static T TryGet<T>(JObject j, string key)  
        {
            JToken t;
            if (j.TryGetValue(key,out t))
                return t.ToObject<T>();
            else
                return default(T);
        }


    }
}