using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : AbstractShipComponent
{
    public override void ApplyShipStatsFromComponent(ShipState shipState)
    {
        base.ApplyShipStatsFromComponent(shipState);

        shipState.HasBridge = true;
    }
}
