using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public struct JobParams 
{
    public float neighborRadius;
    public float separationDistance;
    public Vector3 playerPosition;
    public float deltaTime;
    public float time;

}
