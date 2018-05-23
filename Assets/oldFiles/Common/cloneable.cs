using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if WIN32
#else
using JsonFx;
using JsonFx.Json;
#endif

namespace Helper
{
    static public class SerializeHelper
    {
        public static T Clone<T>(T source)
        {
#if WIN32
            return default(T);
#else
            string data = JsonFx.Json.JsonWriter.Serialize(source);
            return JsonFx.Json.JsonReader.Deserialize<T>(data);
#endif
        }

        public static string ToString<T>(T source)
        {
#if WIN32	
            return null;
#else
            string data = JsonFx.Json.JsonWriter.Serialize(source);
            return data;
#endif
        }
    }


}

