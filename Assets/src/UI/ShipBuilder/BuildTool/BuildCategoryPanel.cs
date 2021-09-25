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

    private float moveTarget = 0;
    private bool willActivate = false;
    private float gottaWait = 0f;

    private void Start()
    {
        moveTarget = transform.localPosition.x;

        var parts = Resources.LoadAll(shipPartSubdirectory, typeof(ShipComponent));
        foreach (var p in parts)
        {
            var buildTool = new BuildTool(100, (ShipComponent)p, PlayerController.Instance.transform);
            _addToolSlot(buildTool);
        }
    }

    private void Update()
    {
        if(gottaWait > 0f)
        {
            gottaWait -= Time.deltaTime;
        }

        if (moveTarget == transform.localPosition.x && gottaWait <= 0f)
        {
            if (willActivate)
            {
                content.gameObject.SetActive(true);
                willActivate = false;
            }
            return;
        }

        var move = MoveRate * Time.deltaTime;

        var nextMove = 0f;

        if (Mathf.Abs((transform.localPosition.x - moveTarget)) <= move)
        {
            nextMove = moveTarget;
        }
        else
        {
            nextMove = transform.localPosition.x + Mathf.Sign(moveTarget - transform.localPosition.x) * move;
        }

        transform.localPosition = new Vector3(nextMove, transform.localPosition.y, transform.localPosition.z);
    }

    public void MoveToAndActivate(float x, bool active)
    {
        moveTarget = x;
        willActivate = active;
        content.gameObject.SetActive(false);

        if(moveTarget == transform.localPosition.x)
        {
            gottaWait = 0.4f;
        }
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
