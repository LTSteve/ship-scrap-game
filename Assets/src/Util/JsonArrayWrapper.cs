using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonArrayWrapper<T>
{
    public T[] Items;

    public static JsonArrayWrapper<T> Wrap(T[] items)
    {
        var toReturn = new JsonArrayWrapper<T>();
        toReturn.Items = items;
        return toReturn;
    }

    public static T[] Unwrap(JsonArrayWrapper<T> wrappedItems)
    {
        return wrappedItems.Items;
    }
}
