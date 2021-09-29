using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using SteveD.TJSON;

public class LinearTestBrain : AbstractBrain
{
    [TextArea(30,300)]
    public string TaskListString;

    private bool initialized = false;

    private List<ITask> taskList = new List<ITask>() {
        new WaitTask(),
        new PrintLineTask(),
        new MultiTask(){ Tasks = new List<ITask> {
            new ThrustTask(),
            new RotationTask()
        } }
    };

    public override ITask GetNextTask()
    {
        if (!initialized)
        {
            if(!string.IsNullOrEmpty(TaskListString))
                taskList = (List<ITask>)TJSONParser.Parse(TaskListString);
            else
            {
                TaskListString = TJSONParser.Encode(taskList);
            }
            initialized = true;
        }

        if(taskList == null || taskList.Count == 0)
        {
            return null;
        }

        var firstTask = taskList[0];
        taskList.RemoveAt(0);
        return firstTask;
    }
}