using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }

    public static void Save(string key = default(string), T data = default(T))
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static void Delete(string key = default(string))
    {
        PlayerPrefs.DeleteKey(key);
    }

    public class SavedDatas : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefsUtil<string>.Save("StringsSaves", "st_Saves");
            PlayerPrefsUtil<int>.Save("ints", 10);
            PlayerPrefsUtil<float>.Save("floats", 10.1f);
            string _load = PlayerPrefsUtil<string>.Load("StringsSaves");
        }

    }
}

