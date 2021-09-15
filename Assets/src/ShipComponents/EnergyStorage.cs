using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyStorage : ShipComponent
{
    public float EnergyCapacity = 1f;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);
        shipState.MaxPower += EnergyCapacity;
    }
}
