using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    [SerializeField]
    public ShipComponent ShipRoot;

    public bool GyrosActive { get; protected set; } = true;

    protected Rigidbody rigidbody;
    protected ShipState myState;
    protected bool control = false;

    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        ToggleControl();
    }

    protected virtual void Update()
    {
        if (!control) return;

        myState.CurrentPower = Mathf.Clamp(myState.CurrentPower + myState.EnergyGeneration * Time.deltaTime, 0f, myState.MaxPower);
    }
    
    protected void _calculateShipState()
    {
        //keep as much power as possible
        var oldPower = myState == null ? 0f : myState.CurrentPower;
        myState = new ShipState();
        myState.CurrentPower = oldPower;

        if (ShipRoot == null)
        {
            var shipComponent = transform.Find("Model").GetComponentInChildren<ShipComponent>();
            if (shipComponent != null)
            {
                var saftey = 1000;
                while (shipComponent.Parent != null)
                {
                    shipComponent = (ShipComponent)shipComponent.Parent;
                    saftey--;
                    if (saftey < 0) break;
                }

                ShipRoot = shipComponent;
            }

            if (ShipRoot == null) return;
        }

        var nodeList = Maths.CreateTreeNodeList(ShipRoot);

        foreach (var node in nodeList)
        {
            var component = (ShipComponent)node;

            component.MyShip = this;
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

        foreach (Transform part in parts)
        {
            foreach (Transform otherpart in parts)
            {
                var dist = Vector3.Distance(part.position, otherpart.position);

                if (dist > maxDist)
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

        foreach (Transform myPart in myParts)
        {
            Physics.IgnoreCollision(myPart.GetComponentInChildren<Collider>(), part.GetComponentInChildren<Collider>(), true);
        }
    }


    public virtual void ToggleControl()
    {
        control = !control;

        if (control) //grab camera
        {
            _calculateShipState();
            myState.Paused = false;
        }
        else
        {
            myState.Paused = true;
        }
    }
}
