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

    public void SetReference(Transform positionRef, Transform aimRef, bool alignToZ = true, bool tight = false)
    {
        positionReference = positionRef;
        aimReference = aimRef;

        transform.position = positionRef.position;
        transform.LookAt(aimRef);

        alignCamToZ = alignToZ;
        tightRotation = tight;

        tilt = Vector2.zero;
    }

    public void Tilt(Vector2 tilt)
    {
        this.tilt = tilt;
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

        var quartb4 = transform.rotation;
        if (alignCamToZ)
        {
            transform.LookAt(aimReference, aimReference.up);
        }
        else
        {
            transform.LookAt(aimReference, transform.up);
        }

        TiltOffset.localPosition = new Vector3(tilt.x, tilt.y);

        if(!tightRotation)
            transform.rotation = Quaternion.Lerp(quartb4, transform.rotation, rotationRate);
    }
}
