using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using SteveD.TJSON;

[Serializable]
public class TaskList : ISerializableRoot
{
    [SerializeField]
    public List<ITask> Tasks;
}