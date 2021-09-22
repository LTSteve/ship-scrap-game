using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsTester
{
    public static void Run()
    {
        UnitTester.RunMyTests(typeof(MathsTester));
    }

    private static void _test_RollingModulo()
    {
        UnitTester.Context(typeof(Maths), "RollingModulo");

        UnitTester.Assert(1, 3, 2);
        UnitTester.Assert(0, 2, 2);
        UnitTester.Assert(0, -2, 2);
        UnitTester.Assert(1, -3, 4);
    }

    private static void _test_CardinalizeVector()
    {
        UnitTester.Context(typeof(Maths), "CardinalizeVector");

        UnitTester.Assert(Vector3.up, Vector3.up);
        UnitTester.Assert(Vector3.down, new Vector3(0f, -0.9f, 0.01f));
        UnitTester.Assert(Vector3.forward, new Vector3(0.2f, -0.2f, 0.8f));
        UnitTester.Assert(Vector3.left, new Vector3(-10f, 0f, 0.5f));
    }

    private static void _test_ApplyDeadzone()
    {
        UnitTester.Context(typeof(Maths), "ApplyDeadzone");

        UnitTester.Assert(0f, 0f, 1f);
        UnitTester.Assert(0f, 1f, 2f);
        UnitTester.Assert(0f, -0.01f, 0.1f);
        UnitTester.Assert(0.25f, 0.25f, 0.01f);
        UnitTester.Assert(-0.24f, -0.24f, 0.01f);
    }

    private static void _test_ClampMovementToSphere()
    {
        UnitTester.Context(typeof(Maths), "ClampMovementToSphere", (output, expectedoutput) => {
            var outputVec = (Vector3)output;
            var expectedOutputVec = (Vector3)expectedoutput;

            return Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x);
        });

        UnitTester.Assert(Vector3.up, Vector3.zero, 1f, Vector3.up, Vector3.zero);
        UnitTester.Assert(new Vector3(1f, 1f, 0f).normalized, new Vector3(0f, 0f, 0f), 1f, new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f));
    }

    private static void _test_WorldDirectionToLocalDirection()
    {
        UnitTester.Context(typeof(Maths), "WorldDirectionToLocalDirection", (output, expectedoutput) => {
            var outputVec = (Vector3)output;
            var expectedOutputVec = (Vector3)expectedoutput;

            return Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x) && Mathf.Approximately(outputVec.x, expectedOutputVec.x);
        });

        UnitTester.Assert(new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        UnitTester.Assert(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), Quaternion.Euler(0f, 0f, 90f));
        UnitTester.Assert(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), Quaternion.Euler(0f, 0f, 70f));
    }
}
