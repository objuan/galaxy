#nullable enable
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;



public class ProgressiveSource: SpawnSource
{
    
    public GameObject startGO;

    public int count;
    int toDoCount;

    public Vector3 startPos;
    public Vector3 startDir;
    public float startSpeed = 1;

    public float spawnDeltaTime = 0.1f;

    public void Start()
    {
        toDoCount = count;
        if (startGO != null)
        {
            startPos = startGO.transform.position;
            startDir = startGO.transform.forward;
        }
    }

    public override StartPose? Next()
    {
        if (toDoCount > 0)
        {
            if (Time.time - runTime > spawnDeltaTime)
            {
                runTime = Time.time;
                toDoCount--;
                return new StartPose { position = startPos, direction = startDir };
            }
        }
        return null;
    }
}