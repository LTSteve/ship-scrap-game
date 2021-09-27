using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    private bool selected = false;

    private Transform model;

    public Transform ItemOffset;

    private Quaternion defaultItemRotation;

    private ShipComponent component;

    public void SetComponent(ShipComponent comp)
    {
        component = comp;

        //instantiate only the model
        model = Instantiate(comp.transform.Find("modelscale"), ItemOffset);

        model.transform.GetChild(0).gameObject.layer = gameObject.layer;

        defaultItemRotation = model.rotation;
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
        Messenger.Default.Publish(new BuildItemSelectedPayload { SelectedComponent = component });
    }
}
