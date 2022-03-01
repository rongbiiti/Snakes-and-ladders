using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// APIKey
/// </summary>
public class ApiKey
{
    /// <summary>
    /// コード
    /// </summary>
    public const string Code = "code";

    /// <summary>
    /// メッセージ
    /// </summary>
    public const string Message = "msg";

    /// <summary>
    /// データ
    /// </summary>
    public const string Data = "data";
}

/// <summary>
/// simple access to json
///
/// var json = JsonNode.Parse(jsonString);
/// string foo = json["hoge"][4].Get<string>();
/// </summary>
public class JsonNode : IEnumerable<JsonNode>, IDisposable
{
    object obj;

    public JsonNode(object obj)
    {
        this.obj = obj;
    }

    public void Dispose()
    {
        obj = null;
    }

    public static JsonNode Parse(string json)
    {
        //UnityEngine.Debug.Log("json" + json);
        return new JsonNode(MiniJSON.Json.Deserialize(json));
    }

    public JsonNode this[int i]
    {
        get
        {
            if (obj is IList)
            {
                return new JsonNode(((IList)obj)[i]);
            }
            throw new Exception("Object is not IList : " + obj.GetType().ToString());
        }
    }

    public JsonNode this[string key]
    {
        get
        {
            if (obj is IDictionary)
            {
                return new JsonNode(((IDictionary)obj)[key]);
            }
            throw new Exception("Object is not IDictionary : " + obj.GetType().ToString());
        }
    }

    public int Count
    {
        get
        {
            if (obj is IList)
            {
                return ((IList)obj).Count;
            }
            else if (obj is IDictionary)
            {
                return ((IDictionary)obj).Count;
            }
            else
            {
                return 0;
            }
        }

    }

    public T Get<T>()
    {
        return (T)obj;
    }
    public string GetKey(int index)
    {
        string result = this[index]["key"].Get<string>();
        return result;
    }
    //public static JsonNode GetValue(int index,string text)
    //{
    //    JsonNode json = JsonNode.Parse(text);
    //    string str = json[index]["value"].Get<string>();
    //    JsonNode result = JsonNode.Parse(str);
    //    return result;
    //}

    public static JsonNode GetValue(string text)
    {
        switch (GameInfo.URLType)
        {
            case URLType.Develop:
            case URLType.Info:
            case URLType.StudyCompas:
                JsonNode jsonNode = JsonNode.Parse(text);
                var dataStr = jsonNode[ApiKey.Data].Get<string>();
                var parsedData = JsonNode.Parse(dataStr);
                return parsedData;
            case URLType.Quadra:
                JsonNode json = JsonNode.Parse(text);
                string str = json[0]["value"].Get<string>();
                JsonNode result = JsonNode.Parse(str);
                return result;
        }

        UnityEngine.Debug.LogError("JsonNodeのGetValueでエラーが発生しました");
        return null;
    }

    public static string GetValueStr(int index, string text)
    {
        JsonNode json = JsonNode.Parse(text);
        string str = json[index]["value"].Get<string>();
        return str;
    }
    public static JsonNode GetValue(JsonNode json, int index)
    {
        string str = json[index]["value"].Get<string>();
        JsonNode result = JsonNode.Parse(str);
        return result;
    }
    public IEnumerator<JsonNode> GetEnumerator()
    {
        if (obj is IList)
        {
            foreach (var o in (IList)obj)
            {
                yield return new JsonNode(o);
            }
        }
        else if (obj is IDictionary)
        {
            foreach (var o in (IDictionary)obj)
            {
                yield return new JsonNode(o);
            }
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
