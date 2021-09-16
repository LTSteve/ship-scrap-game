using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedThruster : ShipComponent
{
    public float Thrust = 8f;

    public float EnergyConsumption = 0.25f;

    private ShipState shipState;
    
    [SerializeField]
    private Transform thrustIndicator;

    private Vector3 relativeThrustDirection;

    private string inputToWatch;

    private int inputDirection = 1;

    private float inputState = 0f;

    private Rigidbody shipBody;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);

        this.shipState = shipState;

        relativeThrustDirection = Maths.CardinalizeVector(Quaternion.Inverse(MyShip.ShipRoot.transform.rotation) * thrustIndicator.rotation * Vector3.forward);

        if (relativeThrustDirection == Vector3.up)
        {
            inputToWatch = "Vertical Thrust";
            inputDirection = 1;
        }
        else if (relativeThrustDirection == Vector3.down)
        {
            inputToWatch = "Vertical Thrust";
            inputDirection = -1;
        }
        else if (relativeThrustDirection == Vector3.right)
        {
            inputToWatch = "Horizontal Thrust";
            inputDirection = -1;
        }
        else if (relativeThrustDirection == Vector3.left)
        {
            inputToWatch = "Horizontal Thrust";
            inputDirection = 1;
        }
        else if (relativeThrustDirection == Vector3.forward)
        {
            inputToWatch = "Thrust";
            inputDirection = -1;
        }
        else
        {
            inputToWatch = "Thrust";
            inputDirection = 1;
        }

        shipBody = MyShip.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (inputToWatch == null) return;

        var input = Input.GetAxisRaw(inputToWatch) * inputDirection;

        inputState = input > 0 ? input : 0f;
    }

    private void FixedUpdate()
    {
        if (inputToWatch == null) return;

        var powerDraw = EnergyConsumption * inputState * Time.fixedDeltaTime;
        if(shipState.CurrentPower > powerDraw && inputState != 0)
        {
            shipState.CurrentPower -= powerDraw;

            shipBody.AddForceAtPosition(shipBody.rotation * -relativeThrustDirection * Thrust * Time.deltaTime, thrustIndicator.position, ForceMode.Force);
        }
    }
}
