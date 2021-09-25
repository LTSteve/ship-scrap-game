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

    public Transform ItemOffset;

    [SerializeField]
    public Vector3 defaultItemPosition = new Vector3(0f, 30f, -10f);

    [SerializeField]
    public Vector3 inflatedItemPosition = new Vector3(0f, 45f, -30f);

    private Transform myCamera;

    public void SetModel(Transform Prefab)
    {
        myCamera = Camera.main.transform;

        child = Instantiate(Prefab, ItemOffset);
        
        child.transform.GetChild(0).gameObject.layer = gameObject.layer;

        ItemOffset.localPosition = defaultItemPosition;
        ItemOffset.localScale = Vector3.one * 35f;
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
        var toCamera = Quaternion.identity;// Quaternion.LookRotation((myCamera.position - transform.position).normalized, myCamera.up);

        if (selected)
        {
            child.rotation *= Quaternion.Euler(0, 170f * Time.deltaTime, 0);
            ItemOffset.localScale = Vector3.Lerp(ItemOffset.localScale, Vector3.one * 45f, 2f * Time.deltaTime);
            ItemOffset.localPosition = toCamera * inflatedItemPosition;

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        }
        else
        {
            child.rotation = Quaternion.Lerp(child.rotation, defaultItemRotation, 2f * Time.deltaTime);
            ItemOffset.localScale = Vector3.Lerp(ItemOffset.localScale, Vector3.one * 35f, 2f * Time.deltaTime);
            ItemOffset.localPosition = toCamera * defaultItemPosition;

            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
            myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60f);
        }
    }
}
