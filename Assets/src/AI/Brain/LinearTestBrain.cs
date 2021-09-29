using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using SteveD.TJSON;

public class LinearTestBrain : AbstractBrain
{
    public TJSON TaskListTSON;

    private bool initialized = false;

    private List<ITask> taskList;

    public override ITask GetNextTask()
    {
        if (!initialized)
        {
            if(TaskListTSON != null && !string.IsNullOrEmpty(TaskListTSON.Data))
                taskList = ((TaskList)TJSONParser.Parse(TaskListTSON.Data)).Tasks;

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