using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractShipComponent : MonoBehaviour
{
    protected Ship MyShip;
    protected ShipState shipState;

    public virtual void ApplyShipStatsFromComponent(ShipState state) {
        MyShip = GetComponentInParent<Ship>();
        shipState = state;
    }

    public virtual void GetDataFromComponent(List<ShipPart.ShipComponentData> data) { }


    [ExecuteAlways]
    public virtual void LoadComponentPropertiesFromModel(ShipPartModel model) { }

    public virtual void SaveComponentPropertiesToModel(ShipPartModel model) { }
}