using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MultiTask : ITask
{
    public List<ITask> Tasks = new List<ITask>();

    public bool Done(Ship ship)
    {
        var done = true;

        foreach (var task in Tasks)
        {
            done = done && task.Done(ship);
        }

        return done;
    }

    public bool Update(Ship ship, float delta)
    {
        foreach (var task in Tasks)
        {
            if (task.Done(ship)) continue;

            task.Update(ship, delta);
        }

        return Done(ship);
    }
}