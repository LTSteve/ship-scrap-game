using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TemporaryEffect : MonoBehaviour
{
    public float Timer = 1f;
    public Func<GameObject, IEnumerator> OnDestroy = null;

    private void Start()
    {
        StartCoroutine(_delayedDestroy());
    }

    private IEnumerator _delayedDestroy()
    {
        yield return new WaitForSeconds(Timer);

        if (OnDestroy != null)
            yield return OnDestroy(this.gameObject);

        Destroy(this.gameObject);
    }
}
