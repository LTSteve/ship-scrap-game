using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLaserGun : ShipComponent
{
    public float Damage = 2f;
    public float EnergyConsumption = 2f;

    [SerializeField]
    private Transform gunTip;

    private LineRenderer lineRenderer;
    private ShipState shipState;
    private bool firing;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);

        this.shipState = shipState;
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override List<ShipComponentData> GetData()
    {
        var toReturn = base.GetData();

        toReturn.Add(new ShipComponentData { Label = "E/s", Value = "" + EnergyConsumption });
        toReturn.Add(new ShipComponentData { Label = "DPS", Value = "" + Damage });

        return toReturn;
    }


    private void Update()
    {
        if (MyShip == null) return;

        firing = MyShip.InputState.Fire1; //joystick 'b' rn
    }

    private void FixedUpdate()
    {
        if (shipState == null || shipState.Paused) return;

        var powerConsumption = EnergyConsumption * Time.deltaTime;

        var potentialTargetedPosition = TargetIndicator.GetTargetedPosition();
        var targetedPosition = potentialTargetedPosition.HasValue ?
            potentialTargetedPosition.Value : gunTip.forward * 1000f + gunTip.position;

        if (!firing || shipState.CurrentPower < powerConsumption)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        shipState.CurrentPower -= powerConsumption;

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, targetedPosition);

        var direction = (targetedPosition - gunTip.position);

        //todo test for collisions and damage
        if (Physics.Raycast(new Ray(gunTip.position, direction.normalized), out var raycastHit, direction.magnitude, new LayerMask() | (1 << LayerMask.NameToLayer("ShipPart")))) //layer 6 is shippart
        {
            var shipPart = raycastHit.collider.transform.parent.GetComponent<ShipComponent>();

            shipPart.DrainHealth(Damage * Time.deltaTime);
        }
    }
}
