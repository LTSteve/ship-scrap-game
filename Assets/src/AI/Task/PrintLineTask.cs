using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class PrintLineTask : ITask
{
    public string Text;

    private bool done = false;

    public bool Done(Ship ship)
    {
        return done;
    }

    public bool Update(Ship ship, float delta)
    {
        done = true;

        return Done(ship);
    }
}