using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static int RollingModulo(int input, int modulo)
    {
        while(input < 0)
        {
            input += modulo;
        }

        return input % modulo;
    }

    public static ITreeNode[] CreateTreeNodeList(ITreeNode root)
    {
        var nodeList = new List<ITreeNode>();

        nodeList.Add(root);

        var childrenList = (root.Children != null) ? new List<ITreeNode>(root.Children) : new List<ITreeNode>();

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
        return value < -deadzone ? value : (value > deadzone ? value : 0f);
    }
}
