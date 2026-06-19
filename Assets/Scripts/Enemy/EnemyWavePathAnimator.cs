
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Enemy))]
public class EnemyWavePathAnimator : MonoBehaviour
{
    Enemy enemy;

    float speed = 200;

    public EnemyWave wave;
    public EnemyPath path;
    public EnemyWaveDefCell cell;

    private void OnEnable()
    {
        enemy = GetComponent<Enemy>();   
    }
    IEnumerator Start()
    {
        int lastFixedPoint = path.lastFixedPoint;

        Vector3 midPoint = wave.GetPivot(cell);
        Vector3 endPoint = wave.LocalToWorld(cell);

        yield return Enter(
            midPoint,
            0,
            4);

        yield return Enter(
            endPoint,
            4,
            path.points.Count - 1);
    }

    System.Collections.IEnumerator Enter(Vector3 endPoint, int fromPoint,int toPoint)
    {
        //EnemyWaveGroup group = wave.waveDef.groups[enter_path.group_idx];

        var subPath = path.GetSubPath(fromPoint, toPoint);

        Vector3 startPoint = transform.position;
       
        Vector2 localStart = path.points[fromPoint+1];
        Vector2 localEnd = path.points[toPoint - 1];

        Vector3 localDir = new Vector3(
            localEnd.x - localStart.x,
            0,
            -(localEnd.y - localStart.y)
        );


        Vector3 worldDir = endPoint - startPoint;
        float scale = worldDir.magnitude / localDir.magnitude;


        Quaternion rot =
            Quaternion.FromToRotation(
                localDir.normalized,
                worldDir.normalized
            );

        float st = Time.time;

        //path.BuildLengthTable();

        while (true)
        {
            var t = Time.time - st;

            float distance = speed * t;

            Vector2? p = subPath.EvaluateSinglePathDistance(distance);

            if (!p.HasValue)
                break;

            Vector3 localPos = new Vector3(
                    p.Value.x - localStart.x,
                    0,
                    -(p.Value.y - localStart.y)
                );

            Vector3 worldPos =
                startPoint +
                rot * (localPos * scale);

            transform.position = worldPos;

            yield return null;
        }


    }

}




