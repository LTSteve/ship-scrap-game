using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPointIndicator : MonoBehaviour
{
    public Transform Sphere1;
    public Transform Sphere2;

    public float AnimationSpeed = 1f;

    private void Update()
    {
        _animateSphere(Sphere1);
        _animateSphere(Sphere2);
    }

    private void _animateSphere(Transform sphere)
    {
        sphere.localScale += Vector3.one * AnimationSpeed * Time.deltaTime;
        if(sphere.localScale.x > 1f)
        {
            sphere.localScale = sphere.localScale - Vector3.one;
        }
    }
}
