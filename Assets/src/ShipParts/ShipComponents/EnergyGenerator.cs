using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : AbstractShipComponent
{
    public float EnergyProduction = 1f;

    public override void ApplyShipStatsFromComponent(ShipState shipState)
    {
        base.ApplyShipStatsFromComponent(shipState);
        shipState.EnergyGeneration += EnergyProduction;
    }


    public override void GetDataFromComponent(List<ShipPart.ShipComponentData> data)
    {
        data.Add(new ShipPart.ShipComponentData { Label = "E/s", Value = "" + EnergyProduction });
    }
}
