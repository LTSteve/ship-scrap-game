using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCategoryPanel : MonoBehaviour
{
    private bool open = false;

    [SerializeField]
    private Transform content;

    [SerializeField]
    private ItemSlot ItemSlotPrefab;

    [SerializeField]
    private string shipPartSubdirectory;

    private int selectedIndex = 0;

    public float MoveRate = 600f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        var parts = Resources.LoadAll(shipPartSubdirectory, typeof(ShipComponent));
        foreach (var p in parts)
        {
            var buildTool = new BuildTool(100, (ShipComponent)p, PlayerController.Instance.transform);
            _addToolSlot(buildTool);
        }
    }

    public void MoveToAndActivate(float x, bool active)
    {
        ThingSlider.DoMeASlide(rectTransform, rectTransform.anchoredPosition, new Vector2(x, rectTransform.anchoredPosition.y), MoveRate, () =>
        {
            content.gameObject.SetActive(active);
        });

        content.gameObject.SetActive(false);
    }

    private void _addToolSlot(ITool newTool)
    {
        var newSlot = Instantiate(ItemSlotPrefab, content);

        newSlot.SetModel(newTool.GetModel());
        newSlot.MyTool = newTool;
    }

    //old code, probably doesn't work
    public ITool ChangeSelectedItem(int itemIndex)
    {
        //skip if we're just setting to where we already are
        if (selectedIndex == itemIndex) return content.GetChild(itemIndex).GetComponent<ItemSlot>().MyTool;

        var count = 0;
        ITool activeTool = null;
        foreach (Transform itemSlotTransform in content)
        {
            var itemSlot = itemSlotTransform.GetComponent<ItemSlot>();

            if (itemSlot.IsSelected() && count != itemIndex)
            { //setting to a new tool, deactivate old tool
                itemSlot.MyTool.Deactivate();
            }

            itemSlot.SetSelected(count == itemIndex);
            if (count == itemIndex) activeTool = itemSlot.MyTool;
            count++;
        }

        if (activeTool == null)
        {
            return null;
        }

        //make sure we activate the new tool after disabling the old tool
        activeTool.Activate();
        selectedIndex = itemIndex;

        return activeTool;
    }
}
