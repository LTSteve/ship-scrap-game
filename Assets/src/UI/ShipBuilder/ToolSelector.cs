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

    [SerializeField]
    private List<AbstractToolView> tools;

    private int currentTool = 4; //4 = no tool

    public void SetSelectedTool(int tool)
    {
        if(tool < 0 || tool >= 4 || tool == currentTool)
        {
            tool = 4;
        }

        DpadImage.sprite = Dpads[tool];

        if(currentTool < 4)
            tools[currentTool]?.Disable();

        currentTool = tool;

        if (tool != 4)
        {
            tools[currentTool]?.Enable();
        }
    }
}
