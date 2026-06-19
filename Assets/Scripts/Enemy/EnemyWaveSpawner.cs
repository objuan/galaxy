
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(EnemyWave))]
public class EnemyWaveSpawner : MonoBehaviour
{
    //public float scaleX = 1f;
    //public float scaleZ = 1f;

    EnemyWave wave;

      
    private void OnEnable()
    {
        wave = GetComponent<EnemyWave>();   
    }
    void Start()
    {

        StartCoroutine(Enter(wave.waveDef.enter_paths[0]));
    }

    System.Collections.IEnumerator Enter(EnemyWavePath_Enter enter_path)
    {
        //EnemyWaveGroup group = wave.waveDef.groups[enter_path.group_idx];

        EnemySpawnSourcePoint startPoint = GameObject.FindObjectsByType<EnemySpawnSourcePoint>(FindObjectsSortMode.None)
                .Where(X => X.source == enter_path.spawnSource).FirstOrDefault();

        var enemy_list = wave.enemies.Select(X => X.GetComponent<Enemy>()).Where(X => X.cell.group_idx == enter_path.group_idx).ToList();

        foreach(var e in enemy_list)
            e.OnBeginEnter();

        float dt = enter_path.spawnDelay;
        float time = Time.time;
        while (enemy_list.Count > 0)
        {
            if (Time.time - time >= dt){ 
                time = Time.time;
                enemy_list[0].OnEnter(startPoint, enter_path.path);
                enemy_list.RemoveAt(0);
            }
            yield return null;
        }
        yield return null;
    }

   
}




