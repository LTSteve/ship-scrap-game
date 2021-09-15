using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLaserGun : ShipComponent
{
    public float Damage = 2f;
    public float EnergyConsumption = 2f;

    [SerializeField]
    private Transform crosshairs;
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

        foreach (Transform child in crosshairs)
        {
            child.gameObject.layer = 7;//that's the crosshairs layer
        }
    }

    private void Update()
    {
        firing = Input.GetButton("Fire2"); //joystick 'b' rn
    }

    private void FixedUpdate()
    {
        if (shipState == null || shipState.Paused) return;

        var powerConsumption = EnergyConsumption * Time.deltaTime;

        if (!firing || shipState.CurrentPower < powerConsumption)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        shipState.CurrentPower -= powerConsumption;

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, crosshairs.position);
        
        //todo test for collisions and damage
    }
}
