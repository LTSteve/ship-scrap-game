using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public static ItemUI Instance;

    private bool open = false;

    [SerializeField]
    private HorizontalLayoutGroup group;

    [SerializeField]
    private ItemSlot ItemSlotPrefab;

    private int selectedIndex = -1;

    void Awake()
    {
        Instance = this;

        group.gameObject.SetActive(false);
    }

    public void SetItemUIOpen(bool state)
    {
        open = state;

        group.gameObject.SetActive(open);

        if(open == false)
        {
            selectedIndex = -1;
        }
    }

    public void AddToolSlot(ITool newTool)
    {
        var newSlot = Instantiate(ItemSlotPrefab, group.transform);

        newSlot.SetModel(newTool.GetModel());
        newSlot.MyTool = newTool;
    }

    public ITool ChangeSelectedItem(int toolIndex)
    {
        //skip if we're just setting to where we already are
        if (selectedIndex == toolIndex) return group.transform.GetChild(toolIndex).GetComponent<ItemSlot>().MyTool;

        var count = 0;
        ITool activeTool = null;
        foreach(Transform itemSlotTransform in group.transform)
        {
            var itemSlot = itemSlotTransform.GetComponent<ItemSlot>();

            if(itemSlot.IsSelected() && count != toolIndex)
            { //setting to a new tool, deactivate old tool
                itemSlot.MyTool.Deactivate();
            }

            itemSlot.SetSelected(count == toolIndex);
            if (count == toolIndex) activeTool = itemSlot.MyTool;
            count++;
        }

        if(activeTool == null)
        {
            return null;
        }

        //make sure we activate the new tool after disabling the old tool
        activeTool.Activate();
        selectedIndex = toolIndex;

        return activeTool;
    }
}
