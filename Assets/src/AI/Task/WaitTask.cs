using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class WaitTask : ITask
{
    [SerializeField]
    public float Time = 1f;

    private float _time = -1f;

    private bool initialized = false;

    public bool Done(Ship ship)
    {
        return initialized && _time <= 0f;
    }

    public bool Update(Ship ship, float delta)
    {
        if(!initialized)
        {
            _time = Time;
            initialized = true;
        }

        _time -= delta;

        return Done(ship);
    }
}