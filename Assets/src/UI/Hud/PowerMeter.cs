using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMeter : MonoBehaviour
{
    public static PowerMeter Instance;
    private void Awake()
    {
        Instance = this;
        bar = transform.Find("Bar");
        Deactivate();
    }

    private Transform bar;

    public void SetState(float full)
    {
        full = Mathf.Clamp01(full);
        bar.localScale = new Vector3(full, 1f, 1f);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
