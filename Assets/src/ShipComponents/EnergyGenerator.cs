using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : ShipComponent
{
    public float EnergyProduction = 1f;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);
        shipState.EnergyGeneration += EnergyProduction;
    }
}
