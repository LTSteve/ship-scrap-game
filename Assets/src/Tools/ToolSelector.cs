using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SuperMaxim.Messaging;

public class ToolSelector : MonoBehaviour
{
    public static ToolSelector Instance;

    private void Awake()
    {
        Instance = this;
    }

    private List<ITool> tools;

    private int currentTool = 4; //4 = no tool

    private void _init()
    {
        tools = new List<ITool>() {
            new BuildTool(),
            new DeleteTool(),
            null,//new ReRootTool()
            null//new ConfigTool()
        };
    }

    public void SetSelectedTool(int tool)
    {
        if (tools == null) _init();

        if (tool < 0 || tool >= 4 || tool == currentTool)
        {
            tool = 4;
        }

        Messenger.Default.Publish(new ToolChangedPayload { Tool = tool });

        if (currentTool < 4)
            tools[currentTool]?.Deactivate();

        currentTool = tool;

        if (tool != 4)
        {
            tools[currentTool]?.Activate();
        }
    }
}
