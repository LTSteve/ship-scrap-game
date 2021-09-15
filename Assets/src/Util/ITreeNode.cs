using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITreeNode
{
    public ShipComponent GetParent();
    public void SetParent(ShipComponent parent);
    public List<ShipComponent> GetChildren();
    public void SetChildren(List<ShipComponent> children);
    public string GetTreeAddress();
}
