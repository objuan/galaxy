using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public struct BoidJob_Progressive : IJobParallelFor
{
    // Dati in lettura
    [ReadOnly] public NativeArray<float> start_time;
    [ReadOnly] public NativeArray<Vector3> positions;
    [ReadOnly] public NativeArray<Vector3> velocities;

    // Parametri
    [ReadOnly] public JobParams pars;

    // Dati in uscita
    public NativeArray<Vector3> accelerations;

    public void Execute(int index)
    {
        Vector3 pos = positions[index];
        Vector3 vel = velocities[index];


       //// acc.y = 0;
       

      
    }
}
