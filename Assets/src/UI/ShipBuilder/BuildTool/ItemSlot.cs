using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemSlot : MonoBehaviour
{
    private bool selected = false;

    public RawImage MyImage;

    private Transform model;

    private Quaternion defaultItemRotation;

    private ShipPart component;

    [SerializeField]
    private ItemInfo myInfo;

    private void Start()
    {
        Messenger.Default.Subscribe<ShipEditorToolInputPayload>(_toggleInfo, (payload) => { return payload.InputType == ShipEditorToolInputPayload.ToolInputType.RightStickClick; });
    }

    public void SetComponent(ShipPart comp)
    {
        component = comp;

        myInfo.Data(component);
        myInfo.gameObject.SetActive(false);

        model = ItemShop.Instance.GetModel(component);

        defaultItemRotation = model.rotation;

        MyImage.uvRect = new Rect(new Vector2(model.localPosition.x, model.localPosition.y) / 10f + new Vector2(0.45f, 0.45f), Vector2.one / 10f);
    }

    private void Update()
    {
        if (selected)
        {
            model.rotation *= Quaternion.Euler(0, 170f * Time.deltaTime, 0);
        }
        else
        {
            model.rotation = Quaternion.Lerp(model.rotation, defaultItemRotation, 2f * Time.deltaTime);
        }
    }

    public void Activate()
    {
        selected = true;

        Messenger.Default.Publish(new BuildItemSelectedPayload { SelectedComponent = component });
    }

    public void Deactivate()
    {
        selected = false;

        myInfo.Disable();
        model.GetComponentInChildren<Renderer>().enabled = true;
    }

    private void _toggleInfo(ShipEditorToolInputPayload payload)
    {
        if (selected)
        {
            var infoActive = myInfo.Toggle();

            model.GetComponentInChildren<Renderer>().enabled = !infoActive;
        }
    }
}
