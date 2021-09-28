using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCam : MonoBehaviour
{
    public static SmoothCam Instance;
    private void Awake()
    {
        Instance = this;
    }

    private Transform positionReference;
    private Transform aimReference;

    public Transform TiltOffset;

    [SerializeField]
    private float minimumMoveDistance = 0.05f;
    [SerializeField]
    private float moveRate = 0.8f;
    [SerializeField]
    private float maxDistance = 0.05f;
    [SerializeField]
    private float rotationRate = 0.8f;

    private bool alignCamToZ = true;
    private bool tightRotation = false;

    private Vector2 tilt = Vector2.zero;

    private Transform temporaryFocusTransform;

    public void SetReference(Transform positionRef, Transform aimRef, bool alignToZ = true, bool tight = false)
    {
        positionReference = positionRef;
        aimReference = aimRef;

        transform.position = positionRef.position;
        transform.LookAt(aimRef);

        alignCamToZ = alignToZ;
        tightRotation = false;//tight;

        tilt = Vector2.zero;
    }

    public void Tilt(Vector2 tilt)
    {
        this.tilt = tilt;
    }

    public void TemporaryFocus(Transform temporaryFocusPoint)
    {
        if(temporaryFocusPoint != temporaryFocusTransform)
        {
            tilt = Vector2.zero;
        }
        temporaryFocusTransform = temporaryFocusPoint;

        if(temporaryFocusPoint == null)
        {
            TiltOffset.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (positionReference == null) return;

        var distanceToMove = positionReference.position - transform.position;

        if(distanceToMove.magnitude >= minimumMoveDistance && !tightRotation)
        {
            var distanceScale = (int)(distanceToMove.magnitude / maxDistance) + 1;
            var willMoveRate = moveRate;

            for(var i = 0; i < distanceScale; i++)
            {
                willMoveRate *= 1f + (1f - willMoveRate);
            }

            transform.position = Vector3.Lerp(transform.position, positionReference.position, willMoveRate);
        }
        else if (tightRotation)
        {
            transform.position = positionReference.position;
        }

        //transform.LookAt(aimReference, transform.up);

        var currentAimReference = (tilt != Vector2.zero || temporaryFocusTransform == null) ? aimReference : temporaryFocusTransform;

        var quartb4 = transform.rotation;
        if (alignCamToZ)
        {
            transform.LookAt(currentAimReference, aimReference.up);
        }
        else
        {
            transform.LookAt(currentAimReference, transform.up);
        }

        TiltOffset.localPosition = Vector3.Lerp(TiltOffset.localPosition, new Vector3(tilt.x, tilt.y), 0.5f * Time.deltaTime);

        if(!tightRotation)
            transform.rotation = Quaternion.Lerp(quartb4, transform.rotation, rotationRate);
    }
}
