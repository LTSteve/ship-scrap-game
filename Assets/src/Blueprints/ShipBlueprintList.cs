using SteveD.TJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ShipBlueprintList : ISerializableRoot
{
    [SerializeField]
    public List<ShipComponentModel> Parts;
}
