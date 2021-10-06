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
    public void SetThrustByString(string input, float value)
    {
        if (input == "Thrust")
        {
            Thrust = value;
        }
        if (input == "Vertical Thrust")
        {
            VerticalThrust = value;
        }
        if (input == "Horizontal Thrust")
        {
            HorizontalThrust = value;
        }
    }
    public void SetRotationByString(string input, float value)
    {
        if (input == "Pitch")
        {
            Pitch = value;
        }
        if (input == "Roll")
        {
            Roll = value;
        }
        if (input == "Yaw")
        {
            Yaw = value;
        }
    }

    public bool Fire1;
    public bool GyrosActive = true;
    public bool IMSActive = true;
}
