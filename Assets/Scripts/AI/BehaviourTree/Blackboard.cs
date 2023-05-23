using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Blackboard
{
    public GridPosition targetPosition;
    [FormerlySerializedAs("playerObject")] public Transform playerTransform;
    public GameObject moveToObject;
}
