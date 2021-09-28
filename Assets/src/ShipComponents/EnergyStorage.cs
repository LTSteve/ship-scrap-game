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

    public override List<ShipComponentData> GetData()
    {
        var toReturn = base.GetData();

        toReturn.Add(new ShipComponentData { Label = "EC", Value = "" + EnergyCapacity });

        return toReturn;
    }
}
