using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyrostabilizer : ShipComponent
{
    public float MaxTorque = 10f;

    public float MaxEnergyDrain = 0.25f;

    public float StabilizationDeadzone = 0.01f;

    private float pitchIn;
    private float rollIn;
    private float yawIn;
    private bool gyrosActive;

    private Rigidbody shipRigidBody;
    private ShipState shipState;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);

        this.shipState = shipState;

        shipRigidBody = MyShip.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (MyShip == null) return;

        var inputs = MyShip.InputState;

        pitchIn = inputs.Pitch;
        rollIn = inputs.Roll;
        yawIn = inputs.Yaw;

        gyrosActive = inputs.GyrosActive;
    }

    private void FixedUpdate()
    {
        if (MyShip == null) return;

        var targetAngularVelocity = new Vector3(-pitchIn, yawIn, -rollIn);

        var gyroTargetAngularVelocity = _generateStabilizationAngularVelocity();

        var torqueTarget = targetAngularVelocity * MaxTorque + gyroTargetAngularVelocity;

        var energyCapacityUsed = (torqueTarget.magnitude / MaxTorque) * MaxEnergyDrain * Time.fixedDeltaTime;

        if(shipState.CurrentPower < energyCapacityUsed)
        {
            torqueTarget = torqueTarget.normalized * (shipState.CurrentPower * MaxTorque) / (MaxEnergyDrain * Time.fixedDeltaTime);
        }

        shipState.CurrentPower -= energyCapacityUsed;

        shipRigidBody.AddRelativeTorque(torqueTarget * Time.fixedDeltaTime, ForceMode.Force);
    }

    public override List<ShipComponentData> GetData()
    {
        var toReturn = base.GetData();

        toReturn.Add(new ShipComponentData { Label = "E/s", Value = "" + MaxEnergyDrain });
        toReturn.Add(new ShipComponentData { Label = "Tq", Value = "" + MaxTorque });

        return toReturn;
    }

    private Vector3 _generateStabilizationAngularVelocity()
    {

        if (!gyrosActive) return Vector3.zero;

        var currentAngularVelocity = shipRigidBody.transform.InverseTransformDirection(shipRigidBody.angularVelocity).normalized * shipRigidBody.angularVelocity.magnitude;

        var counterRotation = Vector3.zero;

        if(pitchIn == 0)
        {
            counterRotation += Vector3.right * -Maths.ApplyDeadzone(currentAngularVelocity.x, StabilizationDeadzone) * 100f;
        }
        if(rollIn == 0)
        {
            counterRotation += Vector3.forward * -Maths.ApplyDeadzone(currentAngularVelocity.z, StabilizationDeadzone) * 100f;
        }
        if(yawIn == 0)
        {
            counterRotation += Vector3.up * -Maths.ApplyDeadzone(currentAngularVelocity.y, StabilizationDeadzone) * 100f;
        }

        var gyroTorque = MaxTorque * 2f;

        //scale down to max torque
        return (counterRotation.magnitude > gyroTorque) ? (counterRotation.normalized * gyroTorque) : counterRotation;
    }
}
