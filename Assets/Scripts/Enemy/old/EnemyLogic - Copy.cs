using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public struct EnemyJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<EnemyData> inData;

    public NativeArray<Vector3> newPos;

   // public EnemyLogic logic;

    public void Execute(int index)
    {
     //   logic.Execute(inData[index]);
    }

}

public struct EnemyJob_Params
{
    public Vector3 targetPosition;
    public float deltaTime;
    public float time;
    public float startTime;
}


public struct EnemyData
{
    public Vector3 targetPosition;
    //public Vector3 position;
    //public Vector3 dir;
    // public float speed;
    // public bool visible;
    public float startTime;
}


// ====

public class EnemyLogic : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float Y = 5;

    public GameObject target;

    public int initialCount;
    //public StartPosMode startPosMode;
    public float linearSpeed = 1;
    public float radialSpeed = 0.1f;
    public float tangentSpeed = 0.1f;

    protected NativeArray<EnemyData> enemyData;

    protected NativeArray<Vector3> position;
    protected NativeArray<float> speed;
    protected NativeArray<bool> visible;

    protected GameObject[] objects;

    protected EnemyJob_Params pars;

    public virtual EnemyData Spawn(EnemyData enemy, int index)
    {
        return enemy;
    }
    /*
    public virtual void Execute(EnemyData enemy)
    {
        Debug.Log(enemy);
    }
    */
    public virtual void Execute(int index)
    {

    }

    public virtual void OnStart()
    {
    }

    public void Start()
    {
        OnStart();

        enemyData = new NativeArray<EnemyData>(initialCount, Allocator.Persistent);
        position = new NativeArray<Vector3>(initialCount, Allocator.Persistent);
        speed = new NativeArray<float>(initialCount, Allocator.Persistent);
        visible = new NativeArray<bool>(initialCount, Allocator.Persistent);
        objects = new GameObject[initialCount];

        for (int i = 0; i < enemyData.Length; i++)
        {
            visible[i] = false;
            enemyData[i] = Spawn(enemyData[i] , i);
        }

        pars = new EnemyJob_Params()
        {
            deltaTime = Time.deltaTime,
            time = Time.time,
            targetPosition = target.transform.position,
            startTime = Time.time,  
        };
    }

    private void Update()
    {
        pars.deltaTime = Time.deltaTime;
        pars.time = Time.time;
        pars.targetPosition = target.transform.position;
      
        for (int i = 0; i < enemyData.Length; i++)
        {
            Execute(i);

            if (visible[i] && !objects[i])
            {
                GameObject go = Instantiate(enemyPrefab, position[i], Quaternion.identity);
                go.transform.position = new Vector3(go.transform.position.x, Y, go.transform.position.z);
                objects[i] = go;
            }
            if (visible[i])
                objects[i].transform.position = position[i];
        }


        /*
        EnemyJob job = new EnemyJob
        {
            inData = enemyData,
        };

        JobHandle handle = job.Schedule(enemyData.Length, 64);
        handle.Complete();
        */
    }

    void OnDestroy()
    {
        if (enemyData.IsCreated) enemyData.Dispose();
        if (position.IsCreated) enemyData.Dispose();
        if (speed.IsCreated) enemyData.Dispose();
        if (visible.IsCreated) enemyData.Dispose();
    }
}

