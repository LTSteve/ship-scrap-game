using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AIShip))]
public abstract class AbstractBrain : MonoBehaviour
{
    public virtual ITask GetNextTask()
    {
        return null;
    }
}