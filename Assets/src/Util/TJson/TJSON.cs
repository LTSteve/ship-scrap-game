using System.Collections;
using UnityEngine;
using System;

namespace SteveD.TJSON
{
    [CreateAssetMenu(fileName = "data", menuName = "TJSON Data")]
    [Serializable]
    public class TJSON : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        public string Data;
    }
}