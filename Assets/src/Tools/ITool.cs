using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool
{
    void Deactivate();
    void Activate();
    void Use();
}
