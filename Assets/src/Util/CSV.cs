using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV
{
    private List<string> data;
    public CSV(string data)
    {
        this.data = new List<string>(data.Split(','));
    }

    public string GetData()
    {
        var nextData = data.Count > 0 ? data[0] : null;
        if (data.Count > 0) {
            data.RemoveAt(0);
        }
        return nextData;
    }
}
