using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class RotationTask : ITask
{
    public float Time = 1f;
    public float Power = 1f;
    public string Rotation = "Pitch";

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

        if(_time > 0)
            ship.InputState.SetRotationByString(Rotation, Power);
        else
            ship.InputState.SetRotationByString(Rotation, 0f);

        return Done(ship);
    }
}