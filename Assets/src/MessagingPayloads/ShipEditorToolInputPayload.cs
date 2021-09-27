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
        RTLT,
        RightStickClick
    }

    public ToolInputType InputType;
    public object InputData;
}