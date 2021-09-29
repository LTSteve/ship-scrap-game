using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static int RollingModulo(int input, int modulo)
    {
        if (modulo == 0) return 0;

        while(input < 0)
        {
            input += modulo;
        }

        return input % modulo;
    }

    //No Tests
    public static ITreeNode[] CreateTreeNodeList(ITreeNode root)
    {
        var nodeList = new List<ITreeNode>();

        nodeList.Add(root);

        var childrenList = (root.GetChildren() != null) ? new List<ShipComponent>(root.GetChildren()) : new List<ShipComponent>();

        var maxIterations = 1000;
        var numIterations = 0;

        while (childrenList.Count != 0)
        {
            childrenList.AddRange(childrenList[0].Children);
            nodeList.Add(childrenList[0]);
            childrenList.RemoveAt(0);

            numIterations++;
            if (numIterations > maxIterations)
            {
                break;
            }
        }

        return nodeList.ToArray();
    }

    public static Vector3 CardinalizeVector(Vector3 vector)
    {
        var absX = vector.x * vector.x;
        var absY = vector.y * vector.y;
        var absZ = vector.z * vector.z;

        if(absX > absY && absX > absZ)
        {
            if(vector.x > 0)
            {
                return Vector3.right;
            }
            else
            {
                return Vector3.left;
            }
        }
        else if(absY > absX && absY > absZ)
        {
            if(vector.y > 0)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.down;
            }
        }
        else
        {
            if(vector.z > 0)
            {
                return Vector3.forward;
            }
            else
            {
                return Vector3.back;
            }
        }
    }

    public static float ApplyDeadzone(float value, float deadzone)
    {
        deadzone = Mathf.Abs(deadzone);

        return value < -deadzone ? value : (value > deadzone ? value : 0f);
    }

    public static Vector3 ClampMovementToSphere(Vector3 sphereCenter, float sphereScale, Vector3 startingPosition, Vector3 desiredMovement)
    {
        var nextPosition = startingPosition + desiredMovement;
        var nextDirection = (nextPosition - sphereCenter).normalized;

        return sphereCenter + nextDirection * sphereScale;
    }
    
    public static Vector3 WorldDirectionToLocalDirection(Vector3 worldDirection, Quaternion subjectRotation)
    {
        return Quaternion.Inverse(subjectRotation) * worldDirection;
    }

    public static Vector4 Q2V4(Quaternion input)
    {
        return new Vector4(input.x, input.y, input.z, input.w);
    }

    public static Quaternion V42Q(Vector4 input)
    {
        return new Quaternion(input.x, input.y, input.z, input.w);
    }
}
