using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Gyrostabilizer
{
    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);

        shipState.HasBridge = true;
    }
}
