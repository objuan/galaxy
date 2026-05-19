using Mono.Cecil;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

// ====

public class EnemyGroup : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float Y = 5;

    public GameObject target;


    public SpawnSource source;

    List<GameObject> list = new List<GameObject>();

    public EnemyCommandStack commandStack;

    public virtual void OnStart()
    {
    }

    public void Start()
    {
        OnStart();

        commandStack.Parse(this);   

        source.Begin();
    }

    private void Update()
    {
        // commands
      
        StartPose pose = null;
        do
        {
            pose = source.Next();
            if (pose != null)
            {
                GameObject go = Instantiate(enemyPrefab, pose.position, Quaternion.identity);
              //  go.SetParentAtOrigin(gameObject);
                go.transform.position = new Vector3(pose.position.x, Y, pose.position.z);
                go.gameObject.AddComponent<Enemy>().commandStack = commandStack;
                
                list.Add(go);
            }
        } while (pose != null);

        

    }

    void OnDestroy()
    {

    }
}

