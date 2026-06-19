
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

    public int group_idx=-1;

}
[Serializable]
public class EnemyWavePath_Enter
{
    public EnemyPath path;

    public EnemySpawnSource spawnSource;

    public int group_idx = -1;

    public float spawnDelay;
}

[Serializable]
public class EnemyWaveGroup
{
    public string name;
    public Color color =Color.white;

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

    public List<EnemyWavePath_Enter> enter_paths = new List<EnemyWavePath_Enter>();

    public List<EnemyWaveGroup> groups = new();

    public void Assign(EnemyWaveDefCell[] cells, EnemyWaveGroup group)
    {
        int groupIdx = groups.IndexOf(group);
        /*
        foreach (EnemyWaveDefCell cell in data)
        {
            if (cell.group_idx == groupIdx)
                cell.group_idx = -1;
        }
        */
        foreach (EnemyWaveDefCell cell in cells)
        {
            cell.group_idx = groupIdx;
        }
    }

    public Vector3 LocalToWorld(Vector3 wavePosition, EnemyWaveDefCell cell)
    {
       return
               wavePosition+ new Vector3(
                    (cell.pos.x - pivot.x) * padding,
                    0,
                    (cell.pos.y - pivot.y) * padding);
    }

}




