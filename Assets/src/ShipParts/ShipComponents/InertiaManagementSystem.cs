using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaManagementSystem : AbstractShipComponent
{
    public float MaxDrag = 0.5f;

    public float MaxEnergyDrain = 0.05f;

    private float thrustIn;
    private float horizontalThrustIn;
    private float verticalThrustIn;
    private bool imsActive;

    private Rigidbody shipRigidBody;

    public override void ApplyShipStatsFromComponent(ShipState shipState)
    {
        base.ApplyShipStatsFromComponent(shipState);

        shipRigidBody = MyShip.GetComponent<Rigidbody>();
    }

    public override void GetDataFromComponent(List<ShipPart.ShipComponentData> data)
    {
        data.Add(new ShipPart.ShipComponentData { Label = "E/s", Value = "" + MaxEnergyDrain });
        data.Add(new ShipPart.ShipComponentData { Label = "I-a", Value = "" + MaxDrag });
    }


    private void Update()
    {
        if (MyShip == null) return;

        var inputs = MyShip.InputState;

        thrustIn = inputs.Thrust;
        horizontalThrustIn = inputs.HorizontalThrust;
        verticalThrustIn = inputs.VerticalThrust;

        imsActive = inputs.IMSActive;
    }

    private void FixedUpdate()
    {
        
        if (MyShip == null || !imsActive) return;

        var currentVelocity = shipRigidBody.velocity;

        if (currentVelocity.magnitude == 0f) return;

        var targetVelocity = MyShip.transform.rotation * new Vector3(horizontalThrustIn, verticalThrustIn, thrustIn);

        var counterVelocityTarget = Maths.ZeroVectorOnAxis(-currentVelocity, targetVelocity);

        if(counterVelocityTarget.magnitude > (currentVelocity.magnitude * 0.5f))
        {
            counterVelocityTarget = counterVelocityTarget.normalized * currentVelocity.magnitude * 0.5f;
        }
        var energyDemand = (counterVelocityTarget.magnitude / currentVelocity.magnitude) * MaxEnergyDrain * Time.deltaTime;

        if (shipState.CurrentPower < energyDemand)
        {
            return;
        }

        shipState.CurrentPower -= energyDemand;

        shipRigidBody.AddForce(counterVelocityTarget * Time.deltaTime, ForceMode.VelocityChange);
    }

}
