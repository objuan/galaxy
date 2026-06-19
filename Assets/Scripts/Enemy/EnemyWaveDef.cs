
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;


[Serializable]
public class EnemyWaveDefCell
{
    public Vector2Int pos;

    // indice della nave
    public int index;

    // traiettoria usata per entrare
    public EnemyPath enter_path;

    public EnemySpawnSource enter_source;

    // ritardo di spawn
    public float spawnDelay;
}

[CreateAssetMenu(
    fileName = "ShipWaveDef",
    menuName = "Game Data/Enemy Wave Def"
)]
public class EnemyWaveDef : ScriptableObject
{
    public string id;
    public string desc;

    public int grid_w = 10;
    public int grid_h = 10;
    public float padding = 1;

    public Vector2Int pivot;

    public List<ShipDef> enemies = new();

    public List<EnemyWaveDefCell> data = new();

    //public List<EnemyPath> paths = new();

    public List<EnemySpawnSource> sources = new();

}




