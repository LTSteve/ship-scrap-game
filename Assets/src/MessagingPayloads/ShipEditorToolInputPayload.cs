using System.Collections;
using UnityEngine;

public class ShipEditorToolInputPayload
{
    public enum ToolInputType
    {
        RightStickHorizontal,
        RBLB,
        B,
        A,
        Y,
        X,
        RT,
        LT,
        RightStickClick
    }

    public ToolInputType InputType;
    public object InputData;
}