using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class UnitTester
{
    private static MethodInfo currentlyTesting;
    private static Func<object, object, bool> currentSuccessCheck;

    public static void RunMyTests(Type testContainer)
    {
        var methods = testContainer.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

        foreach(var method in methods)
        {
            if (!method.Name.StartsWith("_test_")) continue;

            Debug.Log("Testing: " + method.Name.Substring(6) + "...");

            method.Invoke(null, null);
        }
    }

    public static void Context(Type toTest, string methodName, Func<object,object,bool> successCheck = null)
    {
        currentlyTesting = toTest.GetMethod(methodName);
        currentSuccessCheck = successCheck;
    }

    public static void Assert(object expectedOutput, params object[] inputs)
    {
        var output = currentlyTesting.Invoke(null, inputs);

        if((currentSuccessCheck != null && currentSuccessCheck(output, expectedOutput)) || object.Equals(output,expectedOutput))
        {
            Debug.Log(_buildMethodCall(currentlyTesting.Name, inputs) + " = " + expectedOutput + " ... SUCCESS");
        }
        else
        {
            Debug.LogError(_buildMethodCall(currentlyTesting.Name, inputs) + " = " + expectedOutput + " ... FAIL [" + output + "]");
        }
    }

    private static string _buildMethodCall(string name, object[] inputs)
    {
        var toReturn = name + "(";

        for(var i = 0; i < inputs.Length; i++)
        {
            if(i != 0)
            {
                toReturn += ", ";
            }

            toReturn += inputs[i];
        }

        return toReturn + ")";
    }

    public static bool VectorTester(object expectedOutput, object output)
    {
        var outputVec = (Vector3)output;
        var expectedOutputVec = (Vector3)expectedOutput;

        return Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x);
    }

    public static bool VeryApproximateVectorTester(object expectedOutput, object output)
    {
        var outputVec = (Vector3)output;
        var expectedOutputVec = (Vector3)expectedOutput;

        return _approximately(outputVec.x, expectedOutputVec.x) && _approximately(outputVec.x, expectedOutputVec.x) && _approximately(outputVec.x, expectedOutputVec.x);
    }

    private static bool _approximately(float a, float b)
    {
        return a > (b - 0.1f) && a < (b + 0.1f);
    }
}
