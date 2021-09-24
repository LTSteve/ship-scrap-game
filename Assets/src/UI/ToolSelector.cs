using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelector : MonoBehaviour
{
    public static ToolSelector Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private List<Sprite> Dpads;

    [SerializeField]
    private Image DpadImage;

    //private List<ToolUI> tools;
    private int currentTool;

    public void SetSelectedTool(int tool)
    {
        if(tool < 0 || tool >= 4 || tool == currentTool)
        {
            tool = 4;
        }

        DpadImage.sprite = Dpads[tool];

        //tools[currentTool]?.Deactivate();
        if (tool != 4)
        {
            //tools[currentTool] = tools[tool];
            //tools[currentTool].Activate();
        }

        currentTool = tool;
    }

    /*
    public void RegisterTool(int index, ToolUI tool)
    {
        while(tools.Count < index)
        {
            tools.Add(tool);
        }

        tools[index] = tool;
    }
    */
}
