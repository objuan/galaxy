#nullable enable

using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public class StartPose
{
    public Vector3 position;
    public Vector3 direction;
}

// ====
public class SpawnSource: MonoBehaviour
{
    protected float runTime = 0;

    public void Begin()
    {
        runTime  =Time.time;
    }
    public virtual StartPose? Next()
    {
        return null;
    }
}
