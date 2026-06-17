
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyWaveDefCell
{
    public Vector2Int pos;
    public int index;
}


[CreateAssetMenu(
    fileName = "EnemyWaveDef",
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

    public List<EnemyDef> enemies = new();

    public List<EnemyWaveDefCell> data = new();

}


