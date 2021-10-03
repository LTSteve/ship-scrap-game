using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITreeNode
{
    public ShipPart GetParent();
    public void SetParent(ShipPart parent);
    public List<ShipPart> GetChildren();
    public void SetChildren(List<ShipPart> children);
    public string GetTreeAddress();
}
