using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public void Initialize(string message, Color color, float time)
    {
        var textObject = GetComponentInChildren<Text>();
        textObject.text = message;
        textObject.color = color;

        StartCoroutine(_waitAndDisappear(time));
    }

    private IEnumerator _waitAndDisappear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
