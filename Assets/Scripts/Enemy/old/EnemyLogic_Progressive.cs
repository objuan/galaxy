using System;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;



public class EnemyLogic_Progressive : EnemyLogic
{
    /*
    public struct EnemyJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<EnemyData> inData;

        public NativeArray<Vector3> newPos;

        public void Execute(int index)
        {
        }

    }
    */

    public GameObject startGO;

    public Vector3 startPos;
    public Vector3 startDir;
    public float startSpeed=1;

    public float spawnDeltaTime = 0.1f;

    public override void OnStart()
    {
        if (startGO != null)
        {
            startPos = startGO.transform.position;
            startDir = startGO.transform.forward;
        }
    }

    public override EnemyData Spawn(EnemyData enemy, int index)
    {
        position[index] = startPos;
        visible[index] = false;
        speed[index] = startSpeed;
        enemy.startTime = spawnDeltaTime * index;
        return enemy;
    }

    public override void Execute(int index)
    {
        float fromStart = pars.time - pars.startTime;

        if  (!visible[index] && fromStart > enemyData[index].startTime)
        {
            visible[index] = true;
            Debug.Log(index);
        }
        if (visible[index])
        {
            var pos =  position[index];
            var dir = (pos - pars.targetPosition);
            float distance = dir.magnitude;

            var acc = SpiraleToPlayer(position[index], Vector3.zero);
            position[index] += acc * pars.deltaTime * speed[index]; 
        }

       // Debug.Log(index);
    }

    public Vector3 SpiraleToPlayer(Vector3 pos, Vector3 swarmForce)
    {
        Vector3 toPlayer = pars.targetPosition - pos;
        Vector3 radial = toPlayer.normalized;

        // direzione tangenziale (perpendicolare sul piano XZ)
        Vector3 tangent = Vector3.Cross(Vector3.up, radial).normalized;

        // spirale = mix tra entrare e girare
        float spiralStrength = 2.0f;
        float inwardStrength = 0.0f;

        // più sei lontano → più spirale larga
        float dist = toPlayer.magnitude;
        float spiralFactor = Mathf.Clamp01(dist / 10f);

        Vector3 spiralForce = (radial * inwardStrength) + (tangent * spiralStrength * spiralFactor);

        var acc = (swarmForce + spiralForce);
        return acc;
    }

}