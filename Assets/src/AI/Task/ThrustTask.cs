using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ThrustTask : ITask
{
    public float Time = 1f;
    public float Power = 1f;
    public string Direction = "Thrust";

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
            ship.InputState.SetThrustByString(Direction, Power);
        else
            ship.InputState.SetThrustByString(Direction, 0f);

        return Done(ship);
    }
}