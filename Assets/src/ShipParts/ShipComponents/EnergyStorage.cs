using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyStorage : AbstractShipComponent
{
    public float EnergyCapacity = 1f;

    public override void ApplyShipStatsFromComponent(ShipState shipState)
    {
        base.ApplyShipStatsFromComponent(shipState);

        shipState.MaxPower += EnergyCapacity;
    }

    public override void GetDataFromComponent(List<ShipPart.ShipComponentData> data)
    {
        data.Add(new ShipPart.ShipComponentData { Label = "EC", Value = "" + EnergyCapacity });
    }
}
