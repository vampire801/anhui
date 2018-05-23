using UnityEngine;
using System;
using System.Collections.Generic;

using System.IO;
using JsonFx.Json;


public sealed class JsonBase
{
    static JsonReaderSettings readersettings;
    static JsonWriterSettings writesettings;

    static JsonBase()
    {
        writesettings = new JsonWriterSettings();
        writesettings.AddTypeConverter(new Vector2Converter());
        writesettings.AddTypeConverter(new Vector3Converter());
        writesettings.AddTypeConverter(new ColorConverter());

        readersettings = new JsonReaderSettings();
        readersettings.AddTypeConverter(new Vector2Converter());
        readersettings.AddTypeConverter(new Vector3Converter());
        readersettings.AddTypeConverter(new ColorConverter());
    }

    public static T DeserializeFromJson<T>(string jsonData)
    {
        try
        {
            JsonFx.Json.JsonReader reader = new JsonFx.Json.JsonReader(jsonData, readersettings);
            readersettings.AllowNullValueTypes = true;

            T jsonObject = reader.Deserialize<T>();

            return jsonObject;
        }
        catch (Exception ex1)
        {
            Debug.Log(ex1.InnerException.Message);// .printStackTrace();

            return default(T);
        }
        catch
        {
            Debug.Log("Exception " + " " + jsonData);

            return default(T);
        }
    }

    public static void SerializeToJson<T>(string pathfile, object type)
    {
        try
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            JsonFx.Json.JsonWriter writer = new JsonFx.Json.JsonWriter(output, writesettings);
            writer.Write((T)type);
            //File.WriteAllText(pathfile, output.ToString());
        }
        catch (Exception ex1)
        {
            Debug.Log(ex1.InnerException.Message);// .printStackTrace();
        }
        catch
        {
            Debug.Log("SerializeToJson Exception");
        }
    }

    public static string SerializeToString<T>(object type)
    {
        try
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            JsonFx.Json.JsonWriter writer = new JsonFx.Json.JsonWriter(output, writesettings);
            writer.Write((T)type);

            return output.ToString();
        }
        catch
        {
            Debug.Log("SerializeToString Exception");

            return string.Empty;
        }

    }

}

public class Vector3Converter : JsonConverter
{
    public override bool CanConvert(Type t)
    {
        return t == typeof(Vector3);
    }

    public override Dictionary<string, object> WriteJson(Type type, object value)
    {
        Vector3 v = (Vector3)value;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("x", v.x);
        dict.Add("y", v.y);
        dict.Add("z", v.z);
        return dict;
    }

    public override object ReadJson(Type type, Dictionary<string, object> value)
    {
        Vector3 v = new Vector3(CastFloat(value["x"]), CastFloat(value["y"]), CastFloat(value["z"]));
        return v;
    }
}

public class Vector2Converter : JsonConverter
{
    public override bool CanConvert(Type t)
    {
        return t == typeof(Vector2);
    }

    public override Dictionary<string, object> WriteJson(Type type, object value)
    {
        Vector2 v = (Vector2)value;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("x", v.x);
        dict.Add("y", v.y);
        return dict;
    }

    public override object ReadJson(Type type, Dictionary<string, object> value)
    {
        //Debug.Log ("First key type "+value["x"].GetType());
        Vector2 v = new Vector2(CastFloat(value["x"]), CastFloat(value["y"]));
        return v;
    }
}

public class ColorConverter : JsonConverter
{
    public override bool CanConvert(Type t)
    {
        return t == typeof(Color);
    }

    public override Dictionary<string, object> WriteJson(Type type, object value)
    {
        Color v = (Color)value;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("a", v.a);
        dict.Add("r", v.r);
        dict.Add("g", v.g);
        dict.Add("b", v.b);

        return dict;
    }

    public override object ReadJson(Type type, Dictionary<string, object> value)
    {
        //Debug.Log ("First key type "+value["x"].GetType());
        Color v = new Color(CastFloat(value["a"]), CastFloat(value["r"]), CastFloat(value["g"]), CastFloat(value["b"]));

        return v;
    }
}

