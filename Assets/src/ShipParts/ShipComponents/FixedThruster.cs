using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FixedThruster : AbstractShipComponent
{
    public float Thrust = 8f;

    public float EnergyConsumption = 0.25f;

    [SerializeField]
    private Transform thrustIndicator;

    [SerializeField]
    private VisualEffect thrusterSmoke;

    private Vector3 relativeThrustDirection;

    private string inputToWatch;

    private int inputDirection = 1;

    private float inputState = 0f;

    private Rigidbody shipBody;

    public override void ApplyShipStatsFromComponent(ShipState shipState)
    {
        base.ApplyShipStatsFromComponent(shipState);

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

    public override void GetDataFromComponent(List<ShipPart.ShipComponentData> data)
    {
        data.Add(new ShipPart.ShipComponentData { Label = "E/s", Value = "" + EnergyConsumption });
        data.Add(new ShipPart.ShipComponentData { Label = "Tr", Value = "" + Thrust });
    }


    private void Update()
    {
        if (inputToWatch == null) return;

        var input = MyShip.InputState.GetThrustByString(inputToWatch) * inputDirection;

        inputState = input > 0 ? input : 0f;

        if(thrusterSmoke != null)
        {
            thrusterSmoke.SetFloat("Thrust", inputState);
            thrusterSmoke.SetVector3("Global Velocity", shipBody.velocity);
        }
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
