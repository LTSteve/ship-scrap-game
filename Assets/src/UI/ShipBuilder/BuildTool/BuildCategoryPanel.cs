using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperMaxim.Messaging;

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

    [SerializeField]
    private RectTransform highlightSprite;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        var parts = Resources.LoadAll(shipPartSubdirectory, typeof(ShipPart));
        foreach (var p in parts)
        {
            var newSlot = Instantiate(ItemSlotPrefab, content);

            newSlot.SetComponent(((ShipPart)p));
        }
    }

    public void MoveToAndActivate(float x, bool active)
    {
        ThingSlider.DoMeASlide(rectTransform, rectTransform.anchoredPosition, new Vector2(x, rectTransform.anchoredPosition.y), MoveRate, () =>
        {
            SetActive(active);
        });

        //content.gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        //content.gameObject.SetActive(active);
        if (active)
        {
            Messenger.Default.Subscribe<ShipEditorToolInputPayload>(_onInput);
            _setSelectedItem(selectedIndex);
        }
        else
        {
            Messenger.Default.Unsubscribe<ShipEditorToolInputPayload>(_onInput);
        }
    }

    private void _onInput(ShipEditorToolInputPayload payload)
    {
        if (payload.InputType != ShipEditorToolInputPayload.ToolInputType.RightStickHorizontal) return;

        var delta = (float)payload.InputData;

        if (delta > 0)
        {
            selectedIndex = Maths.RollingModulo(selectedIndex + 1, content.childCount);
        }
        if (delta < 0)
        {
            selectedIndex = Maths.RollingModulo(selectedIndex - 1, content.childCount);
        }

        if (delta != 0)
        {
            _setSelectedItem(selectedIndex);
        }
    }
    private void _setSelectedItem(int itemIndex)
    {
        selectedIndex = itemIndex;

        ThingSlider.DoMeASlide(highlightSprite, highlightSprite.anchoredPosition, new Vector2(selectedIndex * 74f, 0f), MoveRate);

        for(var i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i).GetComponent<ItemSlot>();

            if (i == selectedIndex)
                child.Activate();
            else
                child.Deactivate();
        }
    }
}
