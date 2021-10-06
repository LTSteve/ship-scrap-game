using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ToggleControlTask : ITask
{
    [SerializeField]
    public int ToToggle = 0;

    private bool initialized = false;

    public bool Done(Ship ship)
    {
        return initialized;
    }

    public bool Update(Ship ship, float delta)
    {
        if(!initialized)
        {
            initialized = true;
        }

        if(ToToggle == 0)
        {
            ship.InputState.GyrosActive = !ship.InputState.GyrosActive;
        }
        else if(ToToggle == 1)
        {
            ship.InputState.IMSActive = !ship.InputState.IMSActive;
        }

        return Done(ship);
    }
}