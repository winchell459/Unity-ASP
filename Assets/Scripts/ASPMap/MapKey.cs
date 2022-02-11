using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapKey : ScriptableObject
{
    public string widthKey = "width";
    public string heightKey = "height";
    public string typeKey = "tile";

    public int xIndex = 0;
    public int yIndex = 1;
    public int typeIndex = 2;
}

[System.Serializable]
public class MapObjectKey<T>
{
    public MapObject<T>[] mapObjects;
    T FindObject(string key)
    {
        T obj = default;
        foreach(MapObject<T> mapObject in mapObjects)
        {
            if (key == mapObject.key) obj = mapObject.obj;
        }
        return obj;
    }

    public T this[string key]
    {
        get => FindObject(key);
        
    }
}

[System.Serializable]
public class MapObject<T>
{
    public string key;
    public T obj;
}