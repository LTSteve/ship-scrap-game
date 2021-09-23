using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState
{
    public float Pitch;
    public float Roll;
    public float Yaw;

    public float Thrust;
    public float VerticalThrust;
    public float HorizontalThrust;

    public float GetThrustByString(string input)
    {
        if(input == "Thrust")
        {
            return Thrust;
        }
        if(input == "Vertical Thrust")
        {
            return VerticalThrust;
        }
        if(input == "Horizontal Thrust")
        {
            return HorizontalThrust;
        }

        return 0f;
    }

    public bool Fire1;
    public bool GyrosActive = true;
}
