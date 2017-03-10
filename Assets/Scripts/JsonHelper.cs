using UnityEngine;
using System.Collections.Generic;


public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"customers\":" + json + "}";
        Debug.Log(newJson);
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
        return wrapper.array;
    }
 
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}