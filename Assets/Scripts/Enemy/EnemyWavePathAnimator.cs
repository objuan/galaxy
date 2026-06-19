
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(EnemyWave))]
public class EnemyWavePathAnimator : MonoBehaviour
{
    public enum EnemyState
    {
        Entering,
        JoiningFormation,
        InFormation,
        Diving
    }

    public float scaleX = 1f;
    public float scaleZ = 1f;

    EnemyWave wave;

    private float elapsed;
    private Vector3 startPos;

      
    private void OnEnable()
    {
        wave = GetComponent<EnemyWave>();   
    }
    void Start()
    {
        startPos = transform.position;
        elapsed = 0;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        foreach (var go_enemy in wave.enemies)
        {
            var enemy = go_enemy.GetComponent<Enemy>();

            //if (enemy.pathDef != null)
            //{
            //    var p = EvaluatePath(enemy.pathDef, elapsed);
            //    enemy.transform.position = p;   
            //}

        }
       // transform.position = EvaluatePath(path, elapsed);
    }
    /*
    private Vector3 EvaluatePath(EnemyPathDef path, float time)
    {
        if (path.points.Count == 0)
            return Vector3.zero;

        if (time <= path.points[0].time)
        {
            var p = path.points[0];
            return new Vector3(
                p.pos.x * scaleX,
                transform.position.y,
                p.pos.y * scaleZ);
        }

        for (int i = 0; i < path.points.Count - 1; i++)
        {
            var a = path.points[i];
            var b = path.points[i + 1];

            if (time >= a.time && time <= b.time)
            {
                float t = Mathf.InverseLerp(a.time, b.time, time);

                float x = Mathf.Lerp(a.pos.x, b.pos.x, t) * scaleX;
                float z = Mathf.Lerp(a.pos.y, b.pos.y, t) * scaleZ;

                return new Vector3(x, transform.position.y, z);
            }
        }

        var last = path.points[^1];

     
        return new Vector3(
            last.pos.x * scaleX,
            transform.position.y,
            last.pos.y * scaleZ);
    }
    */
}




