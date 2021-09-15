using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    private bool selected = false;

    private Transform child;

    private RectTransform myTransform;

    private Quaternion defaultItemRotation;

    public ITool MyTool;

    public void SetModel(Transform Prefab)
    {
        child = Instantiate(Prefab, transform);
        
        child.transform.GetChild(0).gameObject.layer = gameObject.layer;

        child.localPosition = new Vector3(0, 30f, -30f);
        child.localScale = Vector3.one * 20f;
        child.rotation = Quaternion.Euler(0f, 45f, 0f);
        defaultItemRotation = child.rotation;

        myTransform = GetComponent<RectTransform>();
    }

    public void SetSelected(bool setTo)
    {
        selected = setTo;
    }

    public bool IsSelected()
    {
        return selected;
    }

    private void Update()
    {
        if (selected)
        {
            child.rotation *= Quaternion.Euler(0, 170f * Time.deltaTime, 0);
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 30f, 2f * Time.deltaTime);
            child.localPosition = new Vector3(0, 50f, -30f);

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        }
        else
        {
            child.rotation = Quaternion.Lerp(child.rotation, defaultItemRotation, 2f * Time.deltaTime);
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 20f, 2f * Time.deltaTime);
            child.localPosition = new Vector3(0, 30f, -30f);

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60f);
        }
    }
}
