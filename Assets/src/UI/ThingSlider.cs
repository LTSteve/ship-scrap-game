using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingSlider : MonoBehaviour
{
    public static ThingSlider Instance;

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<RectTransform, Path> transforming = new Dictionary<RectTransform, Path>();

    public static void DoMeASlide(RectTransform me, Vector2 now, Vector2 future, float speed, Action callback = null)
    {
        Instance._doMeASlide(me, now, future, speed, callback);
    }

    private void _doMeASlide(RectTransform me, Vector2 now, Vector2 future, float speed, Action callback)
    {
        if (!transforming.ContainsKey(me))
        {
            transforming.Add(me, new Path());
        }

        var currentPath = transforming[me];

        currentPath.Current = now;
        currentPath.End = future;
        currentPath.Speed = speed;
        currentPath.Callback = callback;
    }

    private void Update()
    {
        var toDelete = new List<RectTransform>();

        foreach (var kvp in transforming)
        {
            var xform = kvp.Key;
            var path = kvp.Value;

            var dist = path.Speed * Time.deltaTime;

            if(dist > Vector2.Distance(path.Current, path.End))
            {
                xform.anchoredPosition = path.End;
                toDelete.Add(xform);
            }
            else
            {
                path.Current += (path.End - path.Current).normalized * dist;
                xform.anchoredPosition = path.Current;
            }
        }

        foreach(var xform in toDelete)
        {
            if (transforming[xform].Callback != null) transforming[xform].Callback();
            transforming.Remove(xform);
        }
    }

    private class Path
    {
        public Vector2 Current;
        public Vector2 End;
        public float Speed;
        public Action Callback;
    }
}
