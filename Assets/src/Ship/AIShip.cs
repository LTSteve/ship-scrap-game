using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShip : Ship
{
    private AbstractBrain myBrain;

    private ITask currentTask;

    private void Awake()
    {
        myBrain = GetComponent<AbstractBrain>();
    }

    protected override void Update()
    {
        base.Update();

        if(currentTask == null)
        {
            currentTask = myBrain.GetNextTask();
        }

        if(currentTask != null && (currentTask.Done(this) || currentTask.Update(this, Time.deltaTime)))
        {
            currentTask = null;
        }
    }
}
