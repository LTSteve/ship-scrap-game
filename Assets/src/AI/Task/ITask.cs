using System.Collections;
using UnityEngine;
using System;

public interface ITask
{
    bool Update(Ship ship, float delta);

    bool Done(Ship ship);
}