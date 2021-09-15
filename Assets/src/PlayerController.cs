using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform CameraPosition;
    [SerializeField]
    private Transform CameraTarget;
    [SerializeField]
    public ShipComponent ShipRoot;

    private Rigidbody rigidbody;

    private bool control = false;

    private ShipState myState;

    private int Scrap = 0;

    public bool GyrosActive { get; private set; } = true;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        ToggleControl();
    }

    void Update()
    {
        if (!control) return;

        if(Input.GetButtonDown("Toggle Gyro"))
        {
            GyrosActive = !GyrosActive;
        }

        myState.CurrentPower = Mathf.Clamp(myState.CurrentPower + myState.EnergyGeneration * Time.deltaTime, 0f, myState.MaxPower);
        PowerMeter.Instance.SetState(myState.MaxPower == 0 ? 0f : (myState.CurrentPower / myState.MaxPower));
    }

    public void AddScrap(int value)
    {
        Scrap += value;
        ScrapCounter.Instance.SetScrap(Scrap);
    }

    public bool PayScrap(int value)
    {
        if(Scrap < value)
        {
            return false;
        }

        Scrap -= value;
        ScrapCounter.Instance.SetScrap(Scrap);

        return true;
    }

    public void ToggleControl()
    {
        control = !control;

        if(control) //grab camera
        {

            SmoothCam.Instance.SetReference(CameraPosition, CameraTarget);

            _calculateShipState();

            PowerMeter.Instance.SetState(myState.MaxPower == 0 ? 0f : (myState.CurrentPower / myState.MaxPower));

            PowerDrawIconBar.Instance.Activate();
            PowerMeter.Instance.Activate();
            myState.Paused = false;
        }
        else
        {
            myState.Paused = true;

            PowerDrawIconBar.Instance.Deactivate();
            PowerMeter.Instance.Deactivate();
        }
    }

    private void _calculateShipState()
    {
        //keep as much power as possible
        var oldPower = myState == null ? 0f : myState.CurrentPower;
        myState = new ShipState();
        myState.CurrentPower = oldPower;

        if (ShipRoot == null)
        {
            return;
        }

        var nodeList = Maths.CreateTreeNodeList(ShipRoot);

        foreach(var node in nodeList)
        {
            var component = (ShipComponent)node;

            component.Player = this;
            component.ApplyShipStats(myState);
        }

        rigidbody.mass = myState.Mass;
        rigidbody.centerOfMass = myState.CenterOfMass;
    }

    //check all parts and return a tip-to-tail distance
    public float GetSize()
    {
        var parts = transform.Find("Model");

        var maxDist = 0f;

        foreach(Transform part in parts)
        {
            foreach(Transform otherpart in parts)
            {
                var dist = Vector3.Distance(part.position,otherpart.position);

                if(dist > maxDist)
                {
                    maxDist = dist;
                }
            }
        }

        return maxDist > 0f ? maxDist : 1f;
    }

    public void IgnoreCollisionsWithPart(ShipComponent part)
    {
        var myParts = transform.Find("Model");

        foreach(Transform myPart in myParts)
        {
            Physics.IgnoreCollision(myPart.GetComponentInChildren<Collider>(), part.GetComponentInChildren<Collider>(), true);
        }
    }
}
