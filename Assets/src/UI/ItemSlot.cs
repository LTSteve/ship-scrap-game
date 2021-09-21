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

    [SerializeField]
    public Vector3 defaultItemPosition = new Vector3(-5f, 40f, -20f);

    [SerializeField]
    public Vector3 inflatedItemPosition = new Vector3(-20f, 60f, -40f);

    public void SetModel(Transform Prefab)
    {
        child = Instantiate(Prefab, transform);
        
        child.transform.GetChild(0).gameObject.layer = gameObject.layer;

        child.localPosition = defaultItemPosition;
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
            child.localPosition = inflatedItemPosition;

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        }
        else
        {
            child.rotation = Quaternion.Lerp(child.rotation, defaultItemRotation, 2f * Time.deltaTime);
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 20f, 2f * Time.deltaTime);
            child.localPosition = defaultItemPosition;

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60f);
        }
    }
}
