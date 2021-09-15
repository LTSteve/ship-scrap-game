using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipState
{
    public bool Paused = false;

    public float MaxPower = 0f;
    public float CurrentPower = 0f;
    public float EnergyGeneration = 0f;

    public bool HasBridge = false;

    public float Mass = 0f;
    public Vector3 CenterOfMass = Vector3.zero;
}
