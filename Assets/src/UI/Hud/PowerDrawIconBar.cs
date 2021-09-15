using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDrawIconBar : MonoBehaviour
{
    public static PowerDrawIconBar Instance;
    private void Awake()
    {
        Instance = this;
        Deactivate();
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
