using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITreeNode
{
    public ITreeNode Parent { get; set; }
    public List<ITreeNode> Children { get; set; }
}
