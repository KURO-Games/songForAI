using UnityEngine;
using System;
[Serializable]
public class JsonClass<T>
{
    public string key;
    public T value;
}
public static class PlayerPrefsUtil<T>
{
    public static bool IsSaved(string key = default(string))
    {
        return PlayerPrefs.HasKey(key);
    }

    public static T Load(string key = default(string), T defaultData = default(T))
    {
        var json = PlayerPrefs.GetString(
            key,
            JsonUtility.ToJson(defaultData)
        );
        var data = JsonUtility.FromJson<JsonClass<T>>(json);
        return data.value;
    }

    public static void Save(string key = default(string), T data = default(T))
    {
        JsonClass<T> jsonData = new JsonClass<T>();
        jsonData.key = key;
        jsonData.value = data;
        var json = JsonUtility.ToJson(jsonData);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static void Delete(string key = default(string))
    {
        PlayerPrefs.DeleteKey(key);
    }
}